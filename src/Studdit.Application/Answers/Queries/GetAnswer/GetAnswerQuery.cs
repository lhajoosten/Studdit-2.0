using Studdit.Application.Answers.Models;
using Studdit.Application.Common.Abstractions;

namespace Studdit.Application.Answers.Queries.GetAnswer
{
    public class GetAnswerQuery : IQuery<AnswerDto>
    {
        public int Id { get; set; }
    }
}