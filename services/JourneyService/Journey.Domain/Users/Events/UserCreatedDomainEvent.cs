using Journey.Domain.Abstractions.Interface;

namespace Journey.Domain.Users.Events;

public record UserCreatedDomainEvent(string email, Guid userId, string token) : IDomainEvent;