using FluentValidation;

namespace Journey.Application.Journys.ShareJourney;

public class ShareJourneyCommandValidator : AbstractValidator<ShareJourneyCommand>
{
    public ShareJourneyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Journey ID must not be empty.");

        RuleFor(x => x.Users)
            .NotEmpty()
            .WithMessage("Users list must not be empty.")
            .Must(users => users.Count <= 10)
            .WithMessage("You can share the journey with a maximum of 10 users.");
    }
}