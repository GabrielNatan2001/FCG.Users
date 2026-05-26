namespace FCG.Users.Application.Messaging;

public interface IMessageBus : IDisposable
{
    void Connect();

    void Publish(string exchange, string routingKey, dynamic command);

    Task Subscribe<TMessage>(
        string exchange,
        string routingKey,
        Func<TMessage, Task> function,
        CancellationToken stoppingToken);
}
