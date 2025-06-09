using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Votes.Commands.UpdateVote
{
    [RequireAuthentication]
    public class UpdateVoteCommand : ICommand
    {
        public int Id { get; set; }
        public string VoteType { get; set; } = string.Empty; // "Upvote" or "Downvote"
    }
}