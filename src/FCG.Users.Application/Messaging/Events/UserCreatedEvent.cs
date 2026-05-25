using MassTransit;

namespace FCG.Users.Application.Messaging.Events;

[EntityName("fcg.user.created")]
public record UserCreatedEvent(Guid UserId, string Email, string Nome, DateTime CreatedAtUtc);
