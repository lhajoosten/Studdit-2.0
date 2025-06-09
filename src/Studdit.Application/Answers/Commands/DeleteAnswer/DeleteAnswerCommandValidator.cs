using FluentValidation;

namespace Studdit.Application.Answers.Commands.DeleteAnswer
{
    public class DeleteAnswerCommandValidator : AbstractValidator<DeleteAnswerCommand>
    {
        public DeleteAnswerCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Answer ID is required.");
        }
    }
}