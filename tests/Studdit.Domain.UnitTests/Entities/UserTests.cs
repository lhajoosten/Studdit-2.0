using FluentAssertions;
using Studdit.Domain.Entities;
using Studdit.Domain.Exceptions;
using Studdit.Domain.ValueObjects;
using Studdit.Test.Utils.Builders;

namespace Studdit.Domain.UnitTests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var username = "testuser";
            var email = Email.Create("test@example.com");
            var passwordHash = "hashedpassword123";
            var displayName = "Test User";

            // Act
            var user = User.Create(username, email, passwordHash, displayName);

            // Assert
            user.Should().NotBeNull();
            user.Username.Should().Be(username);
            user.Email.Should().Be(email);
            user.PasswordHash.Should().Be(passwordHash);
            user.DisplayName.Should().Be(displayName);
            user.Reputation.Should().Be(1);
            user.IsActive.Should().BeTrue();
            user.Bio.Should().BeNull();
            user.LastLoginDate.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidUsername_ShouldThrowException(string username)
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var passwordHash = "hashedpassword123";
            var displayName = "Test User";

            // Act & Assert
            var action = () => User.Create(username, email, passwordHash, displayName);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullEmail_ShouldThrowException()
        {
            // Arrange
            var username = "testuser";
            Email email = null;
            var passwordHash = "hashedpassword123";
            var displayName = "Test User";

            // Act & Assert
            var action = () => User.Create(username, email, passwordHash, displayName);
            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidPasswordHash_ShouldThrowException(string passwordHash)
        {
            // Arrange
            var username = "testuser";
            var email = Email.Create("test@example.com");
            var displayName = "Test User";

            // Act & Assert
            var action = () => User.Create(username, email, passwordHash, displayName);
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidDisplayName_ShouldThrowException(string displayName)
        {
            // Arrange
            var username = "testuser";
            var email = Email.Create("test@example.com");
            var passwordHash = "hashedpassword123";

            // Act & Assert
            var action = () => User.Create(username, email, passwordHash, displayName);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateProfile_WithValidData_ShouldUpdateProfile()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var newDisplayName = "Updated Display Name";
            var newBio = "This is my updated bio";

            // Act
            user.UpdateProfile(newDisplayName, newBio);

            // Assert
            user.DisplayName.Should().Be(newDisplayName);
            user.Bio.Should().Be(newBio);
        }

        [Fact]
        public void UpdateProfile_WithInactiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().Build();
            var newDisplayName = "Updated Display Name";
            var newBio = "This is my updated bio";

            // Act & Assert
            var action = () => user.UpdateProfile(newDisplayName, newBio);
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot update profile of an inactive user.");
        }

        [Fact]
        public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var newEmail = Email.Create("newemail@example.com");

            // Act
            user.UpdateEmail(newEmail);

            // Assert
            user.Email.Should().Be(newEmail);
        }

        [Fact]
        public void UpdateEmail_WithInactiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().Build();
            var newEmail = Email.Create("newemail@example.com");

            // Act & Assert
            var action = () => user.UpdateEmail(newEmail);
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot update email of an inactive user.");
        }

        [Fact]
        public void AddReputation_WithPositivePoints_ShouldIncreaseReputation()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var pointsToAdd = 50;
            var initialReputation = user.Reputation;

            // Act
            user.AddReputation(pointsToAdd);

            // Assert
            user.Reputation.Should().Be(initialReputation + pointsToAdd);
        }

        [Fact]
        public void AddReputation_WithNegativePoints_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var negativePoints = -10;

            // Act & Assert
            var action = () => user.AddReputation(negativePoints);
            action.Should().Throw<DomainException>()
                .WithMessage("Reputation points cannot be negative.");
        }

        [Fact]
        public void ResetReputation_WithActiveUser_ShouldResetToOne()
        {
            // Arrange
            var user = new UserBuilder().WithReputation(500).Build();

            // Act
            user.ResetReputation();

            // Assert
            user.Reputation.Should().Be(1);
        }

        [Fact]
        public void ResetReputation_WithInactiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().Build();

            // Act & Assert
            var action = () => user.ResetReputation();
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot reset reputation of an inactive user.");
        }

        [Fact]
        public void Deactivate_WithActiveUser_ShouldDeactivateUser()
        {
            // Arrange
            var user = new UserBuilder().Build();

            // Act
            user.Deactivate();

            // Assert
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Deactivate_WithInactiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().Build();

            // Act & Assert
            var action = () => user.Deactivate();
            action.Should().Throw<DomainException>()
                .WithMessage("User is already inactive.");
        }

        [Fact]
        public void Activate_WithInactiveUser_ShouldActivateUser()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().Build();

            // Act
            user.Activate();

            // Assert
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Activate_WithActiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().Build();

            // Act & Assert
            var action = () => user.Activate();
            action.Should().Throw<DomainException>()
                .WithMessage("User is already active.");
        }

        [Fact]
        public void UpdateLastLogin_WithActiveUser_ShouldUpdateLastLoginDate()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var beforeUpdate = DateTime.UtcNow;

            // Act
            user.UpdateLastLogin();

            // Assert
            user.LastLoginDate.Should().NotBeNull();
            user.LastLoginDate.Should().BeCloseTo(beforeUpdate, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void UpdateLastLogin_WithInactiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().Build();

            // Act & Assert
            var action = () => user.UpdateLastLogin();
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot update last login date for an inactive user.");
        }

        [Theory]
        [InlineData(15, true)]
        [InlineData(14, false)]
        [InlineData(100, true)]
        public void CanUpvote_WithDifferentReputationLevels_ShouldReturnExpectedResult(int reputation, bool expected)
        {
            // Arrange
            var user = new UserBuilder().WithReputation(reputation).Build();

            // Act
            var result = user.CanUpvote();

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(50, true)]
        [InlineData(49, false)]
        [InlineData(100, true)]
        public void CanComment_WithDifferentReputationLevels_ShouldReturnExpectedResult(int reputation, bool expected)
        {
            // Arrange
            var user = new UserBuilder().WithReputation(reputation).Build();

            // Act
            var result = user.CanComment();

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(125, true)]
        [InlineData(124, false)]
        [InlineData(200, true)]
        public void CanDownvote_WithDifferentReputationLevels_ShouldReturnExpectedResult(int reputation, bool expected)
        {
            // Arrange
            var user = new UserBuilder().WithReputation(reputation).Build();

            // Act
            var result = user.CanDownvote();

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(1500, true)]
        [InlineData(1499, false)]
        [InlineData(2000, true)]
        public void CanCreateTag_WithDifferentReputationLevels_ShouldReturnExpectedResult(int reputation, bool expected)
        {
            // Arrange
            var user = new UserBuilder().WithReputation(reputation).Build();

            // Act
            var result = user.CanCreateTag();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void CanUpvote_WithInactiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().WithReputation(100).Build();

            // Act & Assert
            var action = () => user.CanUpvote();
            action.Should().Throw<DomainException>()
                .WithMessage("Inactive users cannot upvote.");
        }

        [Fact]
        public void DeleteUser_WithActiveUser_ShouldDeactivateAndClearCollections()
        {
            // Arrange
            var user = new UserBuilder().Build();

            // Act
            user.DeleteUser();

            // Assert
            user.IsActive.Should().BeFalse();
            user.Questions.Should().BeEmpty();
            user.Answers.Should().BeEmpty();
            user.Votes.Should().BeEmpty();
        }

        [Fact]
        public void DeleteUser_WithInactiveUser_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().WithInactiveStatus().Build();

            // Act & Assert
            var action = () => user.DeleteUser();
            action.Should().Throw<DomainException>()
                .WithMessage("User is already deleted.");
        }
    }
}