using FluentValidation;

namespace Studdit.Application.Answers.Commands.AcceptAnswer
{
    public class AcceptAnswerCommandValidator : AbstractValidator<AcceptAnswerCommand>
    {
        public AcceptAnswerCommandValidator()
        {
            RuleFor(x => x.AnswerId)
                .GreaterThan(0).WithMessage("Answer ID is required.");
        }
    }
}