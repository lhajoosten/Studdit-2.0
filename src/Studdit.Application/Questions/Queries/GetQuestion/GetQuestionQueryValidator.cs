using FluentValidation;

namespace Studdit.Application.Questions.Queries.GetQuestion
{
    public class GetQuestionQueryValidator : AbstractValidator<GetQuestionQuery>
    {
        public GetQuestionQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Question ID must be greater than 0.");
        }
    }
}
