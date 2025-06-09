using FluentAssertions;
using Studdit.Domain.Exceptions;
using Studdit.Domain.ValueObjects;

namespace Studdit.Domain.UnitTests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_WithValidEmail_ShouldCreateEmail()
        {
            // Arrange
            var emailValue = "test@example.com";

            // Act
            var email = Email.Create(emailValue);

            // Assert
            email.Should().NotBeNull();
            email.Value.Should().Be(emailValue);
        }

        [Fact]
        public void Create_WithValidEmailMixedCase_ShouldNormalizeToLowercase()
        {
            // Arrange
            var emailValue = "Test@EXAMPLE.COM";

            // Act
            var email = Email.Create(emailValue);

            // Assert
            email.Value.Should().Be("test@example.com");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidEmail_ShouldThrowException(string emailValue)
        {
            // Act & Assert
            var action = () => Email.Create(emailValue);
            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test.example.com")]
        [InlineData("test@@example.com")]
        [InlineData("test@.com")]
        [InlineData("test@com")]
        public void Create_WithInvalidEmailFormat_ShouldThrowDomainException(string emailValue)
        {
            // Act & Assert
            var action = () => Email.Create(emailValue);
            action.Should().Throw<DomainException>()
                .WithMessage($"Invalid email format: {emailValue}");
        }

        [Fact]
        public void Create_WithEmailTooLong_ShouldThrowException()
        {
            // Arrange
            var longEmail = new string('a', 310) + "@example.com"; // Total > 320 chars

            // Act & Assert
            var action = () => Email.Create(longEmail);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Equals_WithSameEmailValue_ShouldReturnTrue()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("test@example.com");

            // Act & Assert
            email1.Should().Be(email2);
            (email1 == email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentEmailValues_ShouldReturnFalse()
        {
            // Arrange
            var email1 = Email.Create("test1@example.com");
            var email2 = Email.Create("test2@example.com");

            // Act & Assert
            email1.Should().NotBe(email2);
            (email1 != email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithSameEmailDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("TEST@EXAMPLE.COM");

            // Act & Assert
            email1.Should().Be(email2);
        }

        [Fact]
        public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("test@example.com");

            // Act & Assert
            email1.GetHashCode().Should().Be(email2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnEmailValue()
        {
            // Arrange
            var emailValue = "test@example.com";
            var email = Email.Create(emailValue);

            // Act
            var result = email.ToString();

            // Assert
            result.Should().Be(emailValue);
        }

        [Theory]
        [InlineData("user@domain.com")]
        [InlineData("user.name@domain.com")]
        [InlineData("user+tag@domain.com")]
        [InlineData("user123@sub.domain.com")]
        [InlineData("a@b.co")]
        public void Create_WithValidEmailFormats_ShouldSucceed(string emailValue)
        {
            // Act
            var email = Email.Create(emailValue);

            // Assert
            email.Value.Should().Be(emailValue.ToLowerInvariant());
        }
    }
}