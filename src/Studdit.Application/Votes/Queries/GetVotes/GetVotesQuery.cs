using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Models;
using Studdit.Application.Votes.Models;

namespace Studdit.Application.Votes.Queries.GetVotes
{
    public class GetVotesQuery : IQuery<PaginatedList<VoteSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? UserId { get; set; }
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public string? VoteType { get; set; } // "Upvote", "Downvote"
        public string? SortBy { get; set; } = "CreatedDate";
        public bool SortDescending { get; set; } = true;
    }
}