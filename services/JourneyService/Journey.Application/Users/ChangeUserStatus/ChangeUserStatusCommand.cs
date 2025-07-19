using Journey.Application.Abstractions.Messaging;

namespace Journey.Application.Users.ChangeUserStatus;

public record class ChangeUserStatusCommand(Guid UserId, string Status) : ICommand;