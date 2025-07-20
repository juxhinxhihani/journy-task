using Journey.Domain.Abstractions.Interface;

namespace Journey.Domain.Users.Events;

public record class UserStatusChangedDomainEvent(string email, UserStatus oldStatus, UserStatus newStatus) : IDomainEvent;