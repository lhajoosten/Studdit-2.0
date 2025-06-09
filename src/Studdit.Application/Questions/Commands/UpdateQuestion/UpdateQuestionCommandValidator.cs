using FluentValidation;

namespace Studdit.Application.Questions.Commands.UpdateQuestion
{
    public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
    {
        public UpdateQuestionCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Question ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(10, 150).WithMessage("Title must be between 10 and 150 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MinimumLength(30).WithMessage("Content must be at least 30 characters long.")
                .MaximumLength(10000).WithMessage("Content cannot exceed 10,000 characters.");

            RuleFor(x => x.TagNames)
                .NotEmpty().WithMessage("At least one tag is required.")
                .Must(tags => tags.Count <= 5).WithMessage("Cannot add more than 5 tags.")
                .ForEach(tag => tag.Length(1, 50).WithMessage("Tag name must be between 1 and 50 characters."));
        }
    }
}
