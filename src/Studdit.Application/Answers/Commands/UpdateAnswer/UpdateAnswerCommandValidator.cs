using FluentValidation;

namespace Studdit.Application.Answers.Commands.UpdateAnswer
{
    public class UpdateAnswerCommandValidator : AbstractValidator<UpdateAnswerCommand>
    {
        public UpdateAnswerCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Answer ID is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MinimumLength(30).WithMessage("Content must be at least 30 characters long.")
                .MaximumLength(5000).WithMessage("Content cannot exceed 5,000 characters.");
        }
    }
}