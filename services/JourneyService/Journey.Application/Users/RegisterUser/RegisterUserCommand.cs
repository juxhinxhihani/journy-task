using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs;
using System;

namespace Journey.Application.Users.RegisterUser;

public record class RegisterUserCommand(RegisterUserDTO dto) : ICommand<Guid>;
