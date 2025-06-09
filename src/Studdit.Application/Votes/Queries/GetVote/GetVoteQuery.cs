using Studdit.Application.Common.Abstractions;
using Studdit.Application.Votes.Models;

namespace Studdit.Application.Votes.Queries.GetVote
{
    public class GetVoteQuery : IQuery<VoteDto>
    {
        public int Id { get; set; }
    }
}