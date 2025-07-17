using Journey.Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Journey.Infrastructure.Extensions;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(Error.Failure("Users.Failed", result.Errors.Select(e => e.Description).First()));
    }
}