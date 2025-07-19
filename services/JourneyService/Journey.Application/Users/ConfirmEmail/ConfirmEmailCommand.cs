using Journey.Application.Abstractions.Messaging;

namespace Journey.Application.Users.ConfirmEmail;

public record class ConfirmEmailCommand(Guid userId, string token) : ICommand;