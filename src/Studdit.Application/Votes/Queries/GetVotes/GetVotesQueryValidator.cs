using FluentValidation;

namespace Studdit.Application.Votes.Queries.GetVotes
{
    public class GetVotesQueryValidator : AbstractValidator<GetVotesQuery>
    {
        public GetVotesQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "CreatedDate", "Type" }.Contains(x))
                .WithMessage("Invalid sort field. Allowed values: CreatedDate, Type.");

            RuleFor(x => x.VoteType)
                .Must(x => string.IsNullOrEmpty(x) || x == "Upvote" || x == "Downvote")
                .WithMessage("Vote type must be either 'Upvote' or 'Downvote'.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0.")
                .When(x => x.UserId.HasValue);

            RuleFor(x => x.QuestionId)
                .GreaterThan(0).WithMessage("Question ID must be greater than 0.")
                .When(x => x.QuestionId.HasValue);

            RuleFor(x => x.AnswerId)
                .GreaterThan(0).WithMessage("Answer ID must be greater than 0.")
                .When(x => x.AnswerId.HasValue);
        }
    }
}