using Journey.Application.Abstractions.DbContext;
using Journey.Application.Abstractions.Messaging;
using Journey.Application.DTOs.Response;
using Journey.Domain.Abstractions;
using Journey.Domain.Identity.Interface;
using Journey.Domain.Users;

namespace Journey.Application.Users.SignIn;

public sealed class SignInCommandHandler(
    IIdentityService _identityService,
    IUserRepository _userRepository,
    IApplicationDbContext _context) : ICommandHandler<SignInCommand, SigninResponse>
{
    private readonly string Bearer = "Bearer";
    private readonly int ExpiresIn = 900;

    public async Task<Result<SigninResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.password))
        {
            return Result.Failure<SigninResponse>(UserErrors.LoginFailed);
        }

        User user = await _userRepository.GetUserByEmailAsync(request.email);

        if (user is null || user.Status != UserStatus.Active)
        {
            return Result.Failure<SigninResponse>(UserErrors.NotFound);
        }

        if (user.IsLocked == true)
        {
            return Result.Failure<SigninResponse>(UserErrors.AccountLocked);
        }

        var identityLoginValue = user.Email;
        var (identityUser, roles) = await _identityService.GetUserAndRolesByEmailAsync(identityLoginValue);

        if (identityUser is null || roles is null)
        {
            return Result.Failure<SigninResponse>(UserErrors.NotFound);
        }

        var result = await _identityService.SignInUser(identityUser, request.password, user.Role, cancellationToken);

        if (result.IsFailure)
        {
            user.RetryLogin(user.LoginRetry);
            _userRepository.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Failure<SigninResponse>(result.Error);
        }
        
        var accessToken = _identityService.CreateJwtAccessToken(user, roles);
        user.UnlockUser();
        _userRepository.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new SigninResponse(Bearer, accessToken, ExpiresIn);
    }
}