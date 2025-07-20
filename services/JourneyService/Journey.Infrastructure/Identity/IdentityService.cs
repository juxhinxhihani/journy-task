using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Journey.Domain.Abstractions;
using Journey.Domain.Configuration;
using Journey.Domain.Email;
using Journey.Domain.Identity.Interface;
using Journey.Domain.Users;
using Journey.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Journey.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<User> _userManager,
    SignInManager<User> _signInManager,
    TimeProvider _timeProvider,
    IOptions<JwtConfiguration> _jwtConfiguration,
    RoleManager<Role> _roleManager,
    IEmailService _emailService
) : IIdentityService
{
    private readonly Error Forbidden = Error.Forbidden(
        "User.Forbidden", "Email or password is incorrect");

    private readonly Error EmailNotConfirmed = Error.Failure(
        "User.EmailNotConfirmed", "Confirm your email to continue");

    private readonly Error RoleNotFound = Error.NotFound(
        "Role.NotFound", "The role specified does not exist");

    private readonly Error PasswordNotCorrect = Error.Forbidden(
        "Password.NotCorrect", "The specified password is not correct");

    // User creation methods
    public async Task<Result<User>> CreateIdentityUser(string firstName, string lastName, DateTime dateOfBirth,
        string email, string password, string role)
    {
        var identityUser = User.Create(firstName, lastName, email, dateOfBirth);

        var result = await _userManager.CreateAsync(identityUser, password);

        result = await _userManager.AddToRoleAsync(identityUser, role);

        if (!result.Succeeded)
        {
            return Result.Failure<User>
                (Error.Failure("User.Failed", result.Errors.Select(e => e.Description).First()));
        }
        else
        {
            return identityUser;
        }
    }

    // User sign in methods
    public async Task<Result> SignInUser(User user, string password, string role, CancellationToken cancellationToken)
    {
        var userManager = await _userManager.FindByNameAsync(user.UserName)
            .ConfigureAwait(false);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        if (!await _userManager.IsEmailConfirmedAsync(user)
                .ConfigureAwait(false))
        {
            return Result.Failure(EmailNotConfirmed);
        }

        var signOutTask = _signInManager.SignOutAsync();
        var signInTask = _signInManager.PasswordSignInAsync(user, password, false, true);
        //var tokenTask = _userManager.GenerateTwoFactorTokenAsync(user, "Email");

        await Task.WhenAll(signOutTask, signInTask)
            .ConfigureAwait(false);

        var signinResult = await signInTask;
        if (!signinResult.Succeeded)
        {
            return Result.Failure(PasswordNotCorrect);
        }

        try
        {
            //var token = await tokenTask;

            // using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            // timeoutCts.CancelAfter(TimeSpan.FromSeconds(10)); // 10 second timeout

            //var emailResult = await _emailService.Send2FAOtp(user.Email, token) .ConfigureAwait(false);

            return Result.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            return Result.Failure("There was an issue sending One-Time Passcode to your email. Please try again!");
        }
    }

    public async Task<Result> ResendOtp(User identityUser, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(identityUser.UserName);

        if (!await _userManager.IsEmailConfirmedAsync(identityUser))
        {
            return Result.Failure(EmailNotConfirmed);
        }

        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        var emailResult = await _emailService.Send2FAOtp(identityUser.Email, token);

        if (emailResult)
            return Result.Success();
        else
            return Result.Failure(Forbidden);
    }

    public string CreateJwtAccessToken(User identityUser, List<string> roles)
    {
        try
        {
            var secret = Encoding.ASCII.GetBytes(_jwtConfiguration.Value.Secret);

            var allClaims = new List<Claim>();

            allClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            allClaims.AddRange(new List<Claim>()
            {
                new("id", identityUser.Id.ToString()),
                new(ClaimTypes.Email, identityUser.Email.ToString()),
                new("firstName", identityUser.FirstName ?? string.Empty),
                new("lastName", identityUser.LastName ?? string.Empty)
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(allClaims),
                Expires =
                    _timeProvider.GetUtcNow().DateTime.Add(TimeSpan.FromMinutes(_jwtConfiguration.Value.LifeSpan)),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var accessToken = tokenHandler.WriteToken(token);

            return accessToken;
        }
        catch (Exception)
        {
            throw;
        }
    }

    // User management methods
    public async Task<User?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }

    public async Task<(User? IdentityUser, List<string?> Roles)> GetUserAndRolesByEmailAsync(string email)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);

        if (identityUser is not null)
        {
            var roles = await _userManager.GetRolesAsync(identityUser);

            return (identityUser, roles.ToList());
        }

        return (null, null);
    }

    // Password management methods
    public async Task<Result> ResetPasswordAsync(User identityUser, string token, string password)
    {
        var result = await _userManager.ResetPasswordAsync(identityUser, token, password);

        return result.ToApplicationResult();
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);

        if (identityUser is null || !await _userManager.IsEmailConfirmedAsync(identityUser))
        {
            return Result.Failure<string>(EmailNotConfirmed);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
        return token;
    }

    public async Task<Result> CheckAndChangePasswordAsync(User identityUser, string oldPassword, string newPassword)
    {
        var passwordVerifiedResult = await _userManager.CheckPasswordAsync(identityUser, oldPassword);

        if (!passwordVerifiedResult)
        {
            return Result.Failure(PasswordNotCorrect);
        }

        var result = await _userManager.ChangePasswordAsync(identityUser, oldPassword, newPassword);

        return result.ToApplicationResult();
    }

    // Email management methods
    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(User identityUser)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
        return token;
    }

    public async Task<Result> ConfirmEmail(User identityUser, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(identityUser, token);

        return result.ToApplicationResult();
    }

    public async Task<Result<string>> UpdateEmail(User identityUser, string newEmail)
    {
        identityUser.EmailConfirmed = false;
        identityUser.Email = newEmail;
        identityUser.NormalizedEmail = newEmail.ToUpper();
        identityUser.UserName = newEmail;
        identityUser.NormalizedUserName = newEmail.ToUpper();

        var updateResult = await _userManager.UpdateAsync(identityUser);

        if (!updateResult.Succeeded)
        {
            return Result.Failure<string>(updateResult.ToApplicationResult().Error);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

        return token;
    }
}