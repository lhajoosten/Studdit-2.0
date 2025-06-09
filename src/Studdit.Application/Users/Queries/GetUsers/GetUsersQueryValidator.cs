using FluentValidation;

namespace Studdit.Application.Users.Queries.GetUsers
{
    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        public GetUsersQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "Username", "DisplayName", "Reputation", "CreatedDate" }.Contains(x))
                .WithMessage("Invalid sort field. Allowed values: Username, DisplayName, Reputation, CreatedDate.");
        }
    }
}
