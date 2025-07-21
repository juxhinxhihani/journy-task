using Journey.Application.Abstractions.Messaging;
using Journey.Domain.Users;

namespace Journey.Application.Users.ChangeUserStatus;

public record class ChangeUserStatusCommand(Guid UserId, UserStatus Status) : ICommand;