using FluentValidation;
using Journey.Application.Exceptions;
using Journey.Domain.Abstractions;
using MediatR;

namespace Journey.Application.Abstractions.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        var context = new ValidationContext<TRequest>(request);

        var validationErrors = _validators
            .Select(v => v.Validate(context))
            .Where(v => v.Errors.Any())
            .SelectMany(v => v.Errors)
            .Select(v => Error.Validation(v.PropertyName, v.ErrorMessage))
            .ToList();

        if (validationErrors.Any())
        {
            throw new CustomValidationException(validationErrors);
        }
        else
        {
            return await next();
        }
    }

}
