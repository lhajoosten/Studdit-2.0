using Studdit.Application.Answers.Models;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Models;

namespace Studdit.Application.Answers.Queries.GetAnswers
{
    public class GetAnswersQuery : IQuery<PaginatedList<AnswerSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? QuestionId { get; set; }
        public int? AuthorId { get; set; }
        public bool? IsAccepted { get; set; }
        public string? SortBy { get; set; } = "CreatedDate";
        public bool SortDescending { get; set; } = false; // Show oldest first by default for answers
    }
}