using FluentValidation;

namespace Journey.Application.Journys.DeleteJourney;

public class DeleteJourneyCommandValidator : AbstractValidator<DeleteJourneyCommand>
{ 
    public DeleteJourneyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Journey ID must not be empty.");
    }
}