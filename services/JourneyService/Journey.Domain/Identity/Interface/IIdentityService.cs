using Journey.Domain.Abstractions;
using Journey.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Journey.Domain.Identity.Interface;

public interface IIdentityService
{
    // User creation methods
    Task<Result<User>> CreateIdentityUser(string firstName, string lastName, DateTime dateOfBirth, string email,
        string password, string role);
    
    // User sign in methods
    Task<Result> SignInUser(User identityUser, string password, string role, CancellationToken cancellationToken);
    string CreateJwtAccessToken(User identityUser, List<string> roles);
    Task<Result> ResendOtp(User user, CancellationToken cancellationToken);

    // Password management methods
    Task<Result> ResetPasswordAsync(User identityUser, string token, string password);
    Task<Result<string>> GeneratePasswordResetTokenAsync(string email);
    Task<Result> CheckAndChangePasswordAsync(User identityUser, string oldPassword, string newPassword);
    
    // Email management methods
    Task<Result<string>> UpdateEmail(User identityUser, string newEmail);
    Task<Result> ConfirmEmail(User identityUser, string token);
    Task<Result<string>> GenerateEmailConfirmationTokenAsync(User identityUser);

    // User management methods
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<(User? IdentityUser, List<string?> Roles)> GetUserAndRolesByEmailAsync(string email);

    //Helper methods
    Task AddRoles();
}