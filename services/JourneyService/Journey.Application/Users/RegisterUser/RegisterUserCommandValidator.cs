using FluentValidation;
using FluentValidation.Validators;

namespace Journey.Application.Users.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.dto.FirstName).NotEmpty();
        RuleFor(x => x.dto.LastName).NotEmpty();
        RuleFor(x => x.dto.DateOfBirth).NotEmpty();
        RuleFor(x => x.dto.Role).NotEmpty();
        RuleFor(x => x.dto.Email).NotEmpty().EmailAddress().WithMessage("Email is not valid")
            .MaximumLength(36).WithMessage("Email must be less than 36 characters")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email format is invalid");
        RuleFor(x => x.dto.Password).NotEmpty();
        RuleFor(x => x.dto.ConfirmPassword).Equal(x => x.dto.Password, StringComparer.Ordinal);
    }
}