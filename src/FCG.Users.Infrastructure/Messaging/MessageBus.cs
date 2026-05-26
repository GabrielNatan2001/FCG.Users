using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FCG.Users.Application.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace FCG.Users.Infrastructure.Messaging;

[ExcludeFromCodeCoverage]
public class MessageBus : IMessageBus
{
    public MessageBus(IConfiguration configuration, ILogger<MessageBus> logger)
    {
        _busConfigs = new MessageBusConfigs
        {
            Host = configuration["MessageBusConfigs:Host"]
                ?? throw new InvalidOperationException("MessageBusConfigs:Host não configurado."),
            RetryCount = int.Parse(configuration["MessageBusConfigs:RetryCount"] ?? "5")
        };

        _logger = logger;
        factory = new ConnectionFactory { Uri = new Uri(_busConfigs.Host) };
    }

    private bool _isConnected = false;
    private IConnection? _connection;
    private readonly List<IModel> _consumerChannels = new();
    private ConnectionFactory factory;

    private readonly IDictionary<string, string> _exchange = new Dictionary<string, string>();
    private readonly IDictionary<string, string> _queues = new Dictionary<string, string>();

    private readonly MessageBusConfigs _busConfigs;
    private readonly ILogger<MessageBus> _logger;

    private IConnection connection
    {
        get
        {
            if (!_isConnected || _connection is null || !_connection.IsOpen)
            {
                Connect();
            }

            return _connection!;
        }
    }

    public void Connect()
    {
        try
        {
            var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>()
            .WaitAndRetry(_busConfigs.RetryCount, op => TimeSpan.FromSeconds(Math.Pow(2, op)), (ex, time) =>
            {
                _logger.LogError(ex, "Couldn't connect to RabbitMQ {Uri}", factory.Uri);
            });

            policy.Execute(() =>
            {
                _connection = factory.CreateConnection();
                _isConnected = true;
            });
        }
        catch (Exception)
        {
            Connect();
        }
    }

    private void DeclareExchange(string exchangeName, IModel channel)
    {
        if (_exchange.ContainsKey(exchangeName)) return;

        channel.ExchangeDeclare(exchange: exchangeName + "-exchange", type: ExchangeType.Direct, durable: true);
        _exchange.Add(exchangeName, exchangeName);
    }

    private void DeclareQueueAndBind(string routingKey, string exchangeName, IModel channel)
    {
        DeclareExchange(exchangeName, channel);

        if (_queues.ContainsKey(routingKey)) return;

        channel.QueueDeclare(queue: routingKey + "-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: routingKey + "-queue", exchange: exchangeName + "-exchange", routingKey: routingKey);
        _queues.Add(routingKey, routingKey);
    }

    public void Publish(string exchange, string routingKey, dynamic command)
    {
        using (var channel = connection.CreateModel())
        {
            DeclareExchange(exchange, channel);

            string commandJson = JsonSerializer.Serialize(command);
            _logger.LogInformation(
                "[PUBLISH] - Exchange: {Exchange} | Type: {Type} | RoutingKey: {RoutingKey} | Message: {Message}",
                exchange, ExchangeType.Direct, routingKey, commandJson);

            var body = Encoding.UTF8.GetBytes(commandJson);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object>
            {
                { "X-Retry-Count", 0 }
            };

            channel.BasicPublish(
                exchange: exchange + "-exchange",
                routingKey: routingKey,
                basicProperties: properties,
                body: body);
        }
    }

    public async Task Subscribe<TMessage>(
        string exchange,
        string routingKey,
        Func<TMessage, Task> function,
        CancellationToken stoppingToken)
    {
        await Task.Yield();

        var consumerChannel = connection.CreateModel();
        _consumerChannels.Add(consumerChannel);

        DeclareQueueAndBind(routingKey, exchange, consumerChannel);

        consumerChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(consumerChannel);
        consumerChannel.BasicConsume(queue: routingKey + "-queue", autoAck: false, consumer: consumer);

        consumer.Received += async (sender, eventArgs) =>
        {
            try
            {
                var messageBody = eventArgs.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(messageBody);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var args = JsonSerializer.Deserialize<TMessage>(messageJson, options);

                _logger.LogInformation(
                    "[SUBSCRIBE] - Exchange: {Exchange} | Type: {Type} | RoutingKey: {RoutingKey} | Message: {Message}",
                    exchange, ExchangeType.Direct, routingKey, messageJson);

                await function(args!);

                consumerChannel.BasicAck(eventArgs.DeliveryTag, true);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(
                    "[SUBSCRIBE][InvalidOperationException] - Exchange: {Exchange} | RoutingKey: {RoutingKey} | OPERAÇÃO CANCELADA: {Exception}",
                    exchange, routingKey, ex);
                consumerChannel.BasicAck(eventArgs.DeliveryTag, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "[SUBSCRIBE][EXCEPTION] - Exchange: {Exchange} | RoutingKey: {RoutingKey} | Exception {Exception}",
                    exchange, routingKey, ex.Message);
                consumerChannel.BasicNack(eventArgs.DeliveryTag, false, true);
            }
        };
    }

    public void Dispose()
    {
        foreach (var channel in _consumerChannels)
        {
            try { channel?.Dispose(); } catch { /* ignore */ }
        }
        _consumerChannels.Clear();
        _connection?.Dispose();
    }
}
