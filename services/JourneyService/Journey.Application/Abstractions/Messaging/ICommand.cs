using Journey.Domain.Abstractions;
using MediatR;

namespace Journey.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{

}
public interface IBaseCommand
{
}