using FCG.Users.Application.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCG.Users.Infrastructure.Messaging;

public class RabbitMqTopologyInitializer : IHostedService
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<RabbitMqTopologyInitializer> _logger;
    private readonly MessageTopologyOptions _options;

    public RabbitMqTopologyInitializer(
        IMessageBus messageBus,
        ILogger<RabbitMqTopologyInitializer> logger,
        IOptions<MessageTopologyOptions> options)
    {
        _messageBus = messageBus;
        _logger = logger;
        _options = options.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.Entries.Count == 0)
            return Task.CompletedTask;

        _messageBus.Connect();

        foreach (var entry in _options.Entries)
        {
            _messageBus.EnsureTopology(entry.Exchange, entry.RoutingKey);
            _logger.LogInformation(
                "Topologia RabbitMQ criada: exchange={Exchange}, routingKey={RoutingKey}",
                entry.Exchange,
                entry.RoutingKey);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
