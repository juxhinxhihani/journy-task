using FluentValidation;

namespace Journey.Application.Journys.ShareJourneyPublicLink;

public class ShareJourneyPublicLinkCommandValidator : AbstractValidator<ShareJourneyPublicLinkCommand>
{ 
    public ShareJourneyPublicLinkCommandValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty()
            .WithMessage("Journey ID must not be empty.");
    }
}