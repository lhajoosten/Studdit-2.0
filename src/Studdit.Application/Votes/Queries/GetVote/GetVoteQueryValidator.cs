using FluentValidation;

namespace Studdit.Application.Votes.Queries.GetVote
{
    public class GetVoteQueryValidator : AbstractValidator<GetVoteQuery>
    {
        public GetVoteQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Vote ID must be greater than 0.");
        }
    }
}