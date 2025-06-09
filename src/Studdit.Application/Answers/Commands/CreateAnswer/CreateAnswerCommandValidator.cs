using FluentValidation;

namespace Studdit.Application.Answers.Commands.CreateAnswer
{
    public class CreateAnswerCommandValidator : AbstractValidator<CreateAnswerCommand>
    {
        public CreateAnswerCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MinimumLength(30).WithMessage("Content must be at least 30 characters long.")
                .MaximumLength(5000).WithMessage("Content cannot exceed 5,000 characters.");

            RuleFor(x => x.QuestionId)
                .GreaterThan(0).WithMessage("Question ID is required.");
        }
    }
}