using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs.Response;

namespace Journey.Application.Users.GetUserById;

public record class GetUserByIdQuery(Guid Id) : IQuery<UserByIdResponse>;