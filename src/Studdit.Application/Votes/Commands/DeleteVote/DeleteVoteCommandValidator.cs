using FluentValidation;

namespace Studdit.Application.Votes.Commands.DeleteVote
{
    public class DeleteVoteCommandValidator : AbstractValidator<DeleteVoteCommand>
    {
        public DeleteVoteCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Vote ID is required.");
        }
    }
}