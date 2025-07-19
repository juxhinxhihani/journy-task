using FluentValidation;

namespace Journey.Application.Journys.GetUserJourneys;

public class GetJourneysByUserQueryValidator : AbstractValidator<GetJourneysByUserQuery>
{
    public GetJourneysByUserQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.");
    }
}