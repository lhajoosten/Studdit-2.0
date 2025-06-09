using FluentValidation;

namespace Studdit.Application.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
    {
        public GetUserProfileQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User ID must be greater than 0.");
        }
    }
}
