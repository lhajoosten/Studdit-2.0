using FluentValidation;

namespace Studdit.Application.Answers.Queries.GetAnswer
{
    public class GetAnswerQueryValidator : AbstractValidator<GetAnswerQuery>
    {
        public GetAnswerQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Answer ID must be greater than 0.");
        }
    }
}