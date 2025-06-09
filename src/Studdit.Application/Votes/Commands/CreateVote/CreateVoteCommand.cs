using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Votes.Commands.CreateVote
{
    [RequireAuthentication]
    public class CreateVoteCommand : ICommand<int>
    {
        public string VoteType { get; set; } = string.Empty; // "Upvote" or "Downvote"
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
    }
}