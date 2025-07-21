using Journey.API.Extensions;
using Journey.Application.DTOs;
using Journey.Application.Users.ChangeUserStatus;
using Journey.Application.Users.ConfirmEmail;
using Journey.Application.Users.GetUserById;
using Journey.Application.Users.GetUsers;
using Journey.Application.Users.RegisterUser;
using Journey.Application.Users.SignIn;
using Journey.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ISender _sender) : ControllerBase
{
    
    [HttpPost("register", Name = "RegisterUser")]
    [AllowAnonymous]
    public async Task<IResult> RegisterUser([FromBody] RegisterUserDTO request)
    {
        var command = new RegisterUserCommand(request);
        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            return Results.Ok(result);
        }
        else
        {
            return result.ToProblemDetails();
        }
    }
    [HttpPost("login", Name = "SignIn")]
    [AllowAnonymous]
    public async Task<IResult> SignIn(string email, string password)
    {
        var command = new SignInCommand(email, password);
        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }
        else
        {
            return result.ToProblemDetails();
        }
    }
    [HttpGet(Name = "GetUsers")]
    public async Task<IResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _sender.Send(query);
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }
        else
        {
            return result.ToProblemDetails();
        }
    }

    [HttpGet("{userId}", Name = "GetUserById")]
    public async Task<IResult> GetUserById(Guid userId)
    {
        var query = new GetUserByIdQuery(userId);
        var result = await _sender.Send(query);
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }
        else
        {
            return result.ToProblemDetails();
        }
    }

    [HttpPut("status", Name = "ChangeUserStatus")]
    public async Task<IResult> ChangeUserStatus(Guid userId, UserStatus status)
    {
        var command = new ChangeUserStatusCommand(userId, status);
        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            return Results.Ok(result);
        }
        else
        {
            return result.ToProblemDetails();
        }
    }

    [HttpPost("email/confirm", Name = "ConfirmEmail")]
    [AllowAnonymous]
    public async Task<IResult> ConfirmEmail([FromQuery] Guid id, [FromQuery] string token)
    {
        var command = new ConfirmEmailCommand(id, token);
        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            return Results.Ok(result);
        }
        else
        {
            return result.ToProblemDetails();
        }
    }
}