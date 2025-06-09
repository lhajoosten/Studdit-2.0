using FluentValidation;

namespace Studdit.Application.Questions.Queries.GetQuestions
{
    public class GetQuestionsQueryValidator : AbstractValidator<GetQuestionsQuery>
    {
        public GetQuestionsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "CreatedAt", "VoteScore", "ViewCount", "Title" }.Contains(x))
                .WithMessage("Invalid sort field. Allowed values: CreatedAt, VoteScore, ViewCount, Title.");

            RuleFor(x => x.AuthorId)
                .GreaterThan(0).WithMessage("Author ID must be greater than 0.")
                .When(x => x.AuthorId.HasValue);
        }
    }
}
