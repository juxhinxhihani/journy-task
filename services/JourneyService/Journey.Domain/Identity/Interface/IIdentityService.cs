using Journey.Domain.Abstractions;
using Journey.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Journey.Domain.Identity.Interface;

public interface IIdentityService
{
    // User creation methods
    Task<Result<IdentityUser>> CreateIdentityUser(string email, string role);
    
    // User sign in methods
    Task<Result> SignInUser(IdentityUser identityUser, string password, string role, CancellationToken cancellationToken);
    string CreateJwtAccessToken(User user, IdentityUser identityUser, List<string> roles);
    Task<Result> ResendOtp(IdentityUser identityUser, CancellationToken cancellationToken);

    // Password management methods
    Task<Result> ResetPasswordAsync(IdentityUser identityUser, string token, string password);
    Task<Result<string>> GeneratePasswordResetTokenAsync(string email);
    Task<Result> CheckAndChangePasswordAsync(IdentityUser identityUser, string oldPassword, string newPassword);
    
    // Email management methods
    Task<Result<string>> UpdateEmail(IdentityUser identityUser, string newEmail);
    Task<Result> ConfirmEmail(IdentityUser identityUser, string token);
    Task<Result<string>> GenerateEmailConfirmationTokenAsync(IdentityUser identityUser);

    // User management methods
    Task<IdentityUser?> GetUserByIdAsync(string userId);
    Task<IdentityUser?> GetUserByEmailAsync(string email);
    Task<(IdentityUser? IdentityUser, List<string?> Roles)> GetUserAndRolesByEmailAsync(string email);

    //Helper methods
    Task AddRoles();
}