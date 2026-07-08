namespace FCG.Users.Infrastructure.Messaging;

public class MessageTopologyOptions
{
    public List<MessageTopologyEntry> Entries { get; set; } = [];
}

public class MessageTopologyEntry
{
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}
