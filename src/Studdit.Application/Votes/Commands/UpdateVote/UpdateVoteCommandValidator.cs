using FluentValidation;

namespace Studdit.Application.Votes.Commands.UpdateVote
{
    public class UpdateVoteCommandValidator : AbstractValidator<UpdateVoteCommand>
    {
        public UpdateVoteCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Vote ID is required.");

            RuleFor(x => x.VoteType)
                .NotEmpty().WithMessage("Vote type is required.")
                .Must(x => x == "Upvote" || x == "Downvote")
                .WithMessage("Vote type must be either 'Upvote' or 'Downvote'.");
        }
    }
}