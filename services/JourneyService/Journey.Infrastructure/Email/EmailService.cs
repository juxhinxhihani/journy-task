using System.Threading;
using System.Threading.Tasks;
using Journey.Domain.Email;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Journey.Infrastructure.Email;

public class EmailService(IEmailSender _emailSender) : IEmailService
{
    public async Task<bool> Send2FAOtp(string email, string token)
    {
        var subject = "Your Two-Factor Authentication Code";
        var body = $"Your OTP code is: {token}";
        await _emailSender.SendEmailAsync(email, subject, body);
        return true;
    }

    public async Task<bool> SendConfirmEmail(string email, string confirmationLink)
    {
        var subject = "Confirm Your Email";
        var body = $"Please confirm your email by clicking the following link: {confirmationLink}";
        await _emailSender.SendEmailAsync(email, subject, body);
        return true;
    }

    public async Task<bool> SendResetPasswordEmail(string email, string resetLink)
    {
        var subject = "Reset Your Password";
        var body = $"You can reset your password by clicking the following link: {resetLink}";
        await _emailSender.SendEmailAsync(email, subject, body);
        return true;
    }

    public async Task<bool> SendForgotPasswordEmail(string email, string resetLink)
    {
        var subject = "Forgot Password Assistance";
        var body = $"If you forgot your password, you can reset it using the following link: {resetLink}";
        await _emailSender.SendEmailAsync(email, subject, body);
        return true;
    }

    public async Task<bool> SendStatusChange(string email, string oldStatus, string newStatus)
    {
        var subject = "Your status has changed";
        var body = $"Your account status has changed from {oldStatus} to {newStatus}. If you did not request this change, please contact support.";
        await _emailSender.SendEmailAsync(email, subject, body);
        return true;    }
}
