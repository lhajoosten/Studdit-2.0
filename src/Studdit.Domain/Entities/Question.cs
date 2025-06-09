using Ardalis.GuardClauses;
using Studdit.Domain.Common;
using Studdit.Domain.Enums;
using Studdit.Domain.Exceptions;

namespace Studdit.Domain.Entities
{
    public class Question : BaseEntity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public User Author { get; private set; }
        public int VoteScore { get; private set; }
        public int ViewCount { get; private set; }
        public bool IsAnswered { get; private set; }
        public bool IsClosed { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public string? ClosureReason { get; private set; }

        private readonly List<Answer> _answers = new();
        public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

        private readonly List<Vote> _votes = new();
        public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();

        private readonly List<Tag> _tags = new();
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        private const int MAX_TITLE_LENGTH = 150;
        private const int MIN_CONTENT_LENGTH = 30;
        private const int MAX_TAGS = 5;

        protected Question() { } // For EF Core

        private Question(string title, string content, User author)
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Title cannot be empty.");
            Guard.Against.NullOrWhiteSpace(content, nameof(content), "Content cannot be empty.");
            Guard.Against.Null(author, nameof(author), "Author cannot be null.");
            Guard.Against.OutOfRange(title.Length, nameof(title), 1, MAX_TITLE_LENGTH, $"Title cannot exceed {MAX_TITLE_LENGTH} characters.");
            Guard.Against.OutOfRange(content.Length, nameof(content), MIN_CONTENT_LENGTH, int.MaxValue, $"Content must be at least {MIN_CONTENT_LENGTH} characters.");

            Title = title;
            Content = content;
            Author = author;
            VoteScore = 0;
            ViewCount = 0;
            IsAnswered = false;
            IsClosed = false;
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
        }

        public static Question Create(string title, string content, User author)
        {
            return new Question(title, content, author);
        }

        public void Update(string title, string content)
        {
            if (IsClosed)
                throw new DomainException("Cannot update a closed question.");

            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Title cannot be empty.");
            Guard.Against.NullOrWhiteSpace(content, nameof(content), "Content cannot be empty.");
            Guard.Against.OutOfRange(title.Length, nameof(title), 1, MAX_TITLE_LENGTH, $"Title cannot exceed {MAX_TITLE_LENGTH} characters.");
            Guard.Against.OutOfRange(content.Length, nameof(content), MIN_CONTENT_LENGTH, int.MaxValue, $"Content must be at least {MIN_CONTENT_LENGTH} characters.");

            Title = title;
            Content = content;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void AddAnswer(Answer answer)
        {
            Guard.Against.Null(answer, nameof(answer), "Answer cannot be null.");
            if (IsClosed)
                throw new DomainException("Cannot add an answer to a closed question.");

            _answers.Add(answer);
            IsAnswered = true;
        }

        public void AddVote(Vote vote)
        {
            Guard.Against.Null(vote, nameof(vote), "Vote cannot be null.");
            var existingVote = _votes.FirstOrDefault(v => v.User == vote.User);

            if (existingVote != null)
            {
                if (existingVote.Type == vote.Type)
                    throw new DomainException("User already voted with the same vote type.");

                _votes.Remove(existingVote);
            }

            _votes.Add(vote);
            UpdateVoteScore();
        }

        private void UpdateVoteScore()
        {
            VoteScore = _votes.Sum(v => v.Type == VoteType.Upvote ? 1 : -1);
        }

        public void AddTag(Tag tag)
        {
            Guard.Against.Null(tag, nameof(tag), "Tag cannot be null.");
            if (_tags.Count >= MAX_TAGS)
                throw new DomainException($"Cannot add more than {MAX_TAGS} tags to a question.");

            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                tag.IncrementUsage();
            }
        }

        public void RemoveTag(Tag tag)
        {
            Guard.Against.Null(tag, nameof(tag), "Tag cannot be null.");
            if (!_tags.Contains(tag))
                throw new DomainException("Tag is not associated with this question.");

            if (_tags.Remove(tag))
            {
                tag.DecrementUsage();
            }
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }

        public void Close(string reason)
        {
            Guard.Against.NullOrWhiteSpace(reason, nameof(reason), "Closure reason cannot be empty.");
            if (IsClosed)
                throw new DomainException("Question is already closed.");

            IsClosed = true;
            ClosedAt = DateTime.UtcNow;
            ClosureReason = reason;
        }

        public void Reopen()
        {
            if (!IsClosed)
                throw new DomainException("Question is not closed.");

            IsClosed = false;
            ClosedAt = null;
            ClosureReason = null;
        }
    }
}