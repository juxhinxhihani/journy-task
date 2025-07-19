using FluentValidation;

namespace Journey.Application.Users.SignIn;

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{ 
    public SignInCommandValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}