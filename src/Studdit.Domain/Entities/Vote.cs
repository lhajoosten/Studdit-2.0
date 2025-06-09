using Ardalis.GuardClauses;
using Studdit.Domain.Common;
using Studdit.Domain.Enums;
using Studdit.Domain.Exceptions;

namespace Studdit.Domain.Entities
{
    public class Vote : BaseEntity
    {
        public VoteType Type { get; private set; }
        public User User { get; private set; }
        public Question? Question { get; private set; }
        public Answer? Answer { get; private set; }

        protected Vote() { } // For EF Core

        private Vote(VoteType type, User user, Question? question, Answer? answer)
        {
            Guard.Against.Null(type, nameof(type), "Vote type cannot be null.");
            Guard.Against.Null(user, nameof(user), "User cannot be null.");

            if (question != null && answer != null)
            {
                throw new DomainException("Vote can only be associated with either a question or an answer, not both.");
            }

            Type = type;
            User = user;
            Question = question;
            Answer = answer;
            LastModifiedDate = DateTime.Now;
            LastModifiedByUserId = user.Id;
        }

        public static Vote CreateForQuestion(VoteType type, User user, Question question)
        {
            Guard.Against.Null(question, nameof(question), "Question cannot be null.");
            return new Vote(type, user, question, null);
        }

        public static Vote CreateForAnswer(VoteType type, User user, Answer answer)
        {
            Guard.Against.Null(answer, nameof(answer), "Answer cannot be null.");
            return new Vote(type, user, null, answer);
        }

        public void ChangeVoteType(User user, VoteType newType)
        {
            Guard.Against.Null(user, nameof(user), "User cannot be null.");
            Guard.Against.Null(newType, nameof(newType), "New vote type cannot be null.");

            if (Type == newType)
                throw new DomainException("Cannot change vote type to the same type.");

            Type = newType;
            LastModifiedDate = DateTime.Now;
            LastModifiedByUserId = user.Id;
        }
    }
}