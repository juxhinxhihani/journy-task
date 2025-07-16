using Journey.Domain.Abstractions;
using MediatR;

namespace Journey.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

public interface IGetAllBaseRequest
{
    public string? SearchKey { get; init; }
    public string? ColumnName { get; init; }
    public bool IsDescending { get; init; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}