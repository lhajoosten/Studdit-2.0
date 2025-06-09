using Ardalis.GuardClauses;
using Studdit.Domain.Common;
using Studdit.Domain.Exceptions;

namespace Studdit.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int UsageCount { get; private set; } 

        private readonly List<Question> _questions = new();
        public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        private const int MAX_NAME_LENGTH = 50;
        private const int MAX_DESCRIPTION_LENGTH = 200;

        protected Tag() { } // For EF Core

        private Tag(string name, string description)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Tag name cannot be empty.");
            Guard.Against.OutOfRange(name.Length, nameof(name), 1, MAX_NAME_LENGTH, $"Tag name cannot exceed {MAX_NAME_LENGTH} characters.");
            Guard.Against.NullOrWhiteSpace(description, nameof(description), "Tag description cannot be empty.");
            Guard.Against.OutOfRange(description.Length, nameof(description), 1, MAX_DESCRIPTION_LENGTH, $"Tag description cannot exceed {MAX_DESCRIPTION_LENGTH} characters.");

            Name = name.ToLowerInvariant(); // Normalize to lowercase for consistency
            Description = description;
            CreatedAt = DateTime.UtcNow;
            UsageCount = 0;
        }

        public static Tag Create(string name, string description)
        {
            return new Tag(name, description);
        }

        public void IncrementUsage()
        {
            UsageCount++;
        }

        public void DecrementUsage()
        {
            if (UsageCount <= 0)
                throw new DomainException("Usage count cannot be negative.");

            UsageCount--;
        }

        public void UpdateDescription(string description)
        {
            Guard.Against.NullOrWhiteSpace(description, nameof(description), "Tag description cannot be empty.");
            Guard.Against.OutOfRange(description.Length, nameof(description), 1, MAX_DESCRIPTION_LENGTH, $"Tag description cannot exceed {MAX_DESCRIPTION_LENGTH} characters.");

            Description = description;
        }

        public void AddQuestion(Question question)
        {
            Guard.Against.Null(question, nameof(question), "Question cannot be null.");
            if (_questions.Contains(question))
                throw new DomainException("Question is already associated with this tag.");

            _questions.Add(question);
            IncrementUsage();
        }

        public void RemoveQuestion(Question question)
        {
            Guard.Against.Null(question, nameof(question), "Question cannot be null.");
            if (!_questions.Contains(question))
                throw new DomainException("Question is not associated with this tag.");

            _questions.Remove(question);
            DecrementUsage();
        }
    }
}