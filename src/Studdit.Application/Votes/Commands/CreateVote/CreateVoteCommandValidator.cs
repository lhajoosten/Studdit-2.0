using FluentValidation;

namespace Studdit.Application.Votes.Commands.CreateVote
{
    public class CreateVoteCommandValidator : AbstractValidator<CreateVoteCommand>
    {
        public CreateVoteCommandValidator()
        {
            RuleFor(x => x.VoteType)
                .NotEmpty().WithMessage("Vote type is required.")
                .Must(x => x == "Upvote" || x == "Downvote")
                .WithMessage("Vote type must be either 'Upvote' or 'Downvote'.");

            RuleFor(x => x)
                .Must(x => (x.QuestionId.HasValue && !x.AnswerId.HasValue) || (!x.QuestionId.HasValue && x.AnswerId.HasValue))
                .WithMessage("Must specify either QuestionId or AnswerId, but not both.");

            RuleFor(x => x.QuestionId)
                .GreaterThan(0).WithMessage("Question ID must be greater than 0.")
                .When(x => x.QuestionId.HasValue);

            RuleFor(x => x.AnswerId)
                .GreaterThan(0).WithMessage("Answer ID must be greater than 0.")
                .When(x => x.AnswerId.HasValue);
        }
    }
}