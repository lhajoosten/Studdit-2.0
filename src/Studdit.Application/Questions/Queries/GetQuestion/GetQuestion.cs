using Studdit.Application.Common.Abstractions;
using Studdit.Application.Questions.Models;

namespace Studdit.Application.Questions.Queries.GetQuestion
{
    public class GetQuestionQuery : IQuery<QuestionDto>
    {
        public int Id { get; set; }
    }
}
