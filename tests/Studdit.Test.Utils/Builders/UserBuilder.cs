using Studdit.Domain.Entities;
using Studdit.Domain.ValueObjects;

namespace Studdit.Test.Utils.Builders
{
    public class UserBuilder
    {
        private string _username = "testuser";
        private Email _email = Email.Create("test@example.com");
        private string _passwordHash = "hashedpassword123";
        private string _displayName = "Test User";
        private int _reputation = 1;
        private bool _isActive = true;

        public UserBuilder WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = Email.Create(email);
            return this;
        }

        public UserBuilder WithPasswordHash(string passwordHash)
        {
            _passwordHash = passwordHash;
            return this;
        }

        public UserBuilder WithDisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public UserBuilder WithReputation(int reputation)
        {
            _reputation = reputation;
            return this;
        }

        public UserBuilder WithInactiveStatus()
        {
            _isActive = false;
            return this;
        }

        public User Build()
        {
            var user = User.Create(_username, _email, _passwordHash, _displayName);

            // Use reflection to set private fields for testing purposes
            if (_reputation != 1)
            {
                user.AddReputation(_reputation - 1);
            }

            if (!_isActive)
            {
                user.Deactivate();
            }

            return user;
        }
    }
}
