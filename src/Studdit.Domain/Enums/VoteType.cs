using Studdit.Domain.Common;

namespace Studdit.Domain.Enums
{
    public class VoteType : Enumeration
    {
        public static VoteType Upvote = new VoteType(1, "Upvote");
        public static VoteType Downvote = new VoteType(2, "Downvote");
        public static VoteType Neutral = new VoteType(3, "Neutral");

        public VoteType(int id, string name) : base(id, name)
        {
        }
    }
}
