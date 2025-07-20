namespace Journey.Domain.Email;

public interface IEmailService
{
    Task<bool> Send2FAOtp(string email, string token);
    Task<bool> SendConfirmEmail(string email, string confirmationLink);
    Task<bool> SendResetPasswordEmail(string email, string resetLink);
    Task<bool> SendForgotPasswordEmail(string email, string resetLink);
    Task<bool> SendStatusChange(string email, string oldStatus, string newStatus);
}