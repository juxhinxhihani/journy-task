using FluentValidation;

namespace Journey.Application.Users.GetUsers;

public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.SearchKey)
            .MaximumLength(100)
            .WithMessage("Search key cannot exceed 100 characters.");

        RuleFor(x => x.ColumnName)
            .MaximumLength(50)
            .WithMessage("Column name cannot exceed 50 characters.");
    }
}