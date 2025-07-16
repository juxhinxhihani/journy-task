using System.Runtime.InteropServices.JavaScript;
using Journey.Domain.Abstractions;

namespace Journey.Application.Exceptions;

public class CustomValidationException : Exception
{
    public CustomValidationException(IEnumerable<Error> errors)
    {
        Errors = errors;
    }
    public IEnumerable<Error> Errors { get; }
}