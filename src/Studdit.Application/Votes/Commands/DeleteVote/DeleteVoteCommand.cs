using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Votes.Commands.DeleteVote
{
    [RequireAuthentication]
    public class DeleteVoteCommand : ICommand
    {
        public int Id { get; set; }
    }
}