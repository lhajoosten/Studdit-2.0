using FluentValidation;

namespace Studdit.Application.Answers.Queries.GetAnswers
{
    public class GetAnswersQueryValidator : AbstractValidator<GetAnswersQuery>
    {
        public GetAnswersQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "CreatedDate", "VoteScore", "IsAccepted" }.Contains(x))
                .WithMessage("Invalid sort field. Allowed values: CreatedDate, VoteScore, IsAccepted.");

            RuleFor(x => x.QuestionId)
                .GreaterThan(0).WithMessage("Question ID must be greater than 0.")
                .When(x => x.QuestionId.HasValue);

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Author ID must be greater than 0.")
                .When(x => x.AuthorId.HasValue);
        }
    }
}