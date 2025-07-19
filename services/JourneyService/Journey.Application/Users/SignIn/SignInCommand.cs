using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs.Response;

namespace Journey.Application.Users.SignIn;

public record class SignInCommand(string email, string password) : ICommand<SigninResponse>;
