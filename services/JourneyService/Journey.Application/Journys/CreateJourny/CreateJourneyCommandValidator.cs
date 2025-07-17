using FluentValidation;

namespace Journey.Application.Journys.AddJourny;

public class CreateJourneyCommandValidator : AbstractValidator<CreateJourneyCommand>
{
    public CreateJourneyCommandValidator()
    {
        RuleFor(x => x.dto)
            .NotNull()
            .WithMessage("Journey request cannot be null.");

        RuleFor(x => x.dto.StartLocation)
            .NotEmpty()
            .WithMessage("Start location cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Start location cannot exceed 100 characters.");

        RuleFor(x => x.dto.StartTime)
            .NotEmpty()
            .WithMessage("Start time cannot be empty.");

        RuleFor(x => x.dto.ArrivalLocation)
            .NotEmpty()
            .WithMessage("Arrival location cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Arrival location cannot exceed 100 characters.");

        RuleFor(x => x.dto.ArrivalTime)
            .GreaterThan(x => x.dto.StartTime)
            .WithMessage("Arrival time must be greater than the start time.");
        
        RuleFor(x => x.dto.RouteDistanceKm)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Route distance must be greater than or equal to 0.");
    }
}