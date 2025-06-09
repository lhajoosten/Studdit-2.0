using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Models;
using Studdit.Application.Questions.Models;

namespace Studdit.Application.Questions.Queries.GetQuestions
{
    public class GetQuestionsQuery : IQuery<PaginatedList<QuestionSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public List<string>? TagNames { get; set; }
        public bool? IsAnswered { get; set; }
        public bool? IsClosed { get; set; }
        public int? AuthorId { get; set; }
        public string? SortBy { get; set; } = "CreatedDate";
        public bool SortDescending { get; set; } = true;
    }
}
