namespace FCG.Users.Application.Messaging;

public class UserCreatedPublisherConfig
{
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}
