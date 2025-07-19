using FluentValidation;

namespace Journey.Application.Journys.RevokePublicLink;

public class RevokeJourneyPublicLinkCommandValidator : AbstractValidator<RevokeJourneyPublicLinkCommand>
{ 
    public RevokeJourneyPublicLinkCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID must not be empty.");
    }
}