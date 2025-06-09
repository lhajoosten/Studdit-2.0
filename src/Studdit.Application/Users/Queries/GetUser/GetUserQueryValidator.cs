using FluentValidation;

namespace Studdit.Application.Users.Queries.GetUser
{
    public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User ID must be greater than 0.");
        }
    }
}
