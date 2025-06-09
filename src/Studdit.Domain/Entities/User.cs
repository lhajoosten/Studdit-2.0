using Ardalis.GuardClauses;
using Studdit.Domain.Common;
using Studdit.Domain.Exceptions;
using Studdit.Domain.ValueObjects;

namespace Studdit.Domain.Entities
{
    public class User : BaseEntity, IAggregateRoot
    {
        public string Username { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }

        public string DisplayName { get; private set; }
        public string? Bio { get; private set; }

        public DateTime? LastLoginDate { get; private set; }
        public int Reputation { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<Question> _questions = new();
        public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        private readonly List<Answer> _answers = new();
        public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

        private readonly List<Vote> _votes = new();
        public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();

        private const int UPVOTE_PRIVILEGE_THRESHOLD = 15;
        private const int COMMENT_PRIVILEGE_THRESHOLD = 50;
        private const int DOWNVOTE_PRIVILEGE_THRESHOLD = 125;
        private const int CREATE_TAG_PRIVILEGE_THRESHOLD = 1500;

        protected User() { } // For EF Core

        private User(string username, Email email, string passwordHash, string displayName)
        {
            Guard.Against.NullOrWhiteSpace(username, nameof(username), "Username cannot be empty.");
            Guard.Against.Null(email, nameof(email), "Email cannot be null.");
            Guard.Against.NullOrWhiteSpace(passwordHash, nameof(passwordHash), "Password cannot be empty.");
            Guard.Against.NullOrWhiteSpace(displayName, nameof(displayName), "Display name cannot be empty.");

            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            DisplayName = displayName;
            Reputation = 1; // Starting reputation
            IsActive = true;
            CreatedDate = DateTime.UtcNow;
        }

        public static User Create(string username, Email email, string passwordHash, string displayName)
        {
            return new User(username, email, passwordHash, displayName);
        }

        public void UpdateProfile(string displayName, string? bio)
        {
            if (!IsActive)
                throw new DomainException("Cannot update profile of an inactive user.");

            Guard.Against.NullOrWhiteSpace(displayName, nameof(displayName), "Display name cannot be empty.");
            DisplayName = displayName;
            Bio = bio;
        }

        public void UpdateEmail(Email email)
        {
            if (!IsActive)
                throw new DomainException("Cannot update email of an inactive user.");

            Guard.Against.Null(email, nameof(email), "Email cannot be null.");
            Email = email;
        }

        public void AddReputation(int points)
        {
            if (points < 0)
                throw new DomainException("Reputation points cannot be negative.");

            Reputation += points;
        }

        public void ResetReputation()
        {
            if (!IsActive)
                throw new DomainException("Cannot reset reputation of an inactive user.");

            Reputation = 1; // Reset to starting reputation
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("User is already inactive.");

            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("User is already active.");

            IsActive = true;
        }

        public void UpdateLastLogin()
        {
            if (!IsActive)
                throw new DomainException("Cannot update last login date for an inactive user.");

            LastLoginDate = DateTime.UtcNow;
        }

        public void DeleteUser()
        {
            if (!IsActive)
                throw new DomainException("User is already deleted.");

            IsActive = false;
            _questions.Clear();
            _answers.Clear();
            _votes.Clear();
        }

        public bool CanUpvote()
        {
            if (!IsActive)
                throw new DomainException("Inactive users cannot upvote.");

            return Reputation >= UPVOTE_PRIVILEGE_THRESHOLD;
        }

        public bool CanComment()
        {
            if (!IsActive)
                throw new DomainException("Inactive users cannot comment.");

            return Reputation >= COMMENT_PRIVILEGE_THRESHOLD;
        }

        public bool CanDownvote()
        {
            if (!IsActive)
                throw new DomainException("Inactive users cannot downvote.");

            return Reputation >= DOWNVOTE_PRIVILEGE_THRESHOLD;
        }

        public bool CanCreateTag()
        {
            if (!IsActive)
                throw new DomainException("Inactive users cannot create tags.");

            return Reputation >= CREATE_TAG_PRIVILEGE_THRESHOLD;
        }
    }
}