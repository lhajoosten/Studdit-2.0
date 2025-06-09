using Ardalis.GuardClauses;
using Studdit.Domain.Common;
using Studdit.Domain.Enums;
using Studdit.Domain.Exceptions;

namespace Studdit.Domain.Entities
{
    public class Answer : BaseEntity
    {
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public User Author { get; private set; }
        public Question Question { get; private set; }
        public int VoteScore { get; private set; }
        public bool IsAccepted { get; private set; }

        private readonly List<Vote> _votes = new();
        public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();

        protected Answer() { } // For EF Core

        private Answer(string content, User author, Question question)
        {
            Guard.Against.NullOrWhiteSpace(content, nameof(content), "Content cannot be empty.");
            Guard.Against.Null(author, nameof(author), "Author cannot be null.");
            Guard.Against.Null(question, nameof(question), "Question cannot be null.");

            Content = content;
            Author = author;
            Question = question;
            CreatedAt = DateTime.UtcNow;
            VoteScore = 0;
            IsAccepted = false;
        }

        public static Answer Create(string content, User author, Question question)
        {
            if (question.IsClosed)
                throw new DomainException("Cannot create an answer for a closed question.");

            return new Answer(content, author, question);
        }

        public void Update(string content)
        {
            if (Question.IsClosed)
                throw new DomainException("Cannot update an answer for a closed question.");

            Guard.Against.NullOrWhiteSpace(content, nameof(content), "Content cannot be empty.");
            Content = content;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsAccepted()
        {
            if (Question.Author != Author)
                throw new DomainException("Only the question author can mark an answer as accepted.");

            IsAccepted = true;
        }

        public void UnmarkAsAccepted()
        {
            if (!IsAccepted)
                throw new DomainException("Answer is not marked as accepted.");

            IsAccepted = false;
        }

        public void AddVote(Vote vote)
        {
            Guard.Against.Null(vote, nameof(vote), "Vote cannot be null.");
            _votes.Add(vote);
            UpdateVoteScore();
        }

        private void UpdateVoteScore()
        {
            VoteScore = _votes.Sum(v => v.Type == VoteType.Upvote ? 1 : -1);
        }
    }
}