namespace FCG.Users.Infrastructure.Messaging;

public class MessageBusConfigs
{
    public string Host { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 5;
}
