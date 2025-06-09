using FluentAssertions;
using Studdit.Domain.Entities;
using Studdit.Domain.Exceptions;
using Studdit.Test.Utils.Builders;

namespace Studdit.Domain.UnitTests.Entities
{
    public class TagTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateTag()
        {
            // Arrange
            var name = "test-tag";
            var description = "This is a test tag description.";

            // Act
            var tag = Tag.Create(name, description);

            // Assert
            tag.Should().NotBeNull();
            tag.Name.Should().Be(name.ToLowerInvariant());
            tag.Description.Should().Be(description);
            tag.UsageCount.Should().Be(0);
            tag.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            tag.Questions.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidName_ShouldThrowException(string name)
        {
            // Arrange
            var description = "This is a test tag description.";

            // Act & Assert
            var action = () => Tag.Create(name, description);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithNameTooLong_ShouldThrowException()
        {
            // Arrange
            var name = new string('a', 51); // Max is 50
            var description = "This is a test tag description.";

            // Act & Assert
            var action = () => Tag.Create(name, description);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidDescription_ShouldThrowException(string description)
        {
            // Arrange
            var name = "test-tag";

            // Act & Assert
            var action = () => Tag.Create(name, description);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithDescriptionTooLong_ShouldThrowException()
        {
            // Arrange
            var name = "test-tag";
            var description = new string('a', 201); // Max is 200

            // Act & Assert
            var action = () => Tag.Create(name, description);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Create_WithMixedCaseName_ShouldNormalizeToLowercase()
        {
            // Arrange
            var name = "Test-TAG";
            var description = "This is a test tag description.";

            // Act
            var tag = Tag.Create(name, description);

            // Assert
            tag.Name.Should().Be("test-tag");
        }

        [Fact]
        public void IncrementUsage_ShouldIncreaseUsageCount()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var initialUsageCount = tag.UsageCount;

            // Act
            tag.IncrementUsage();

            // Assert
            tag.UsageCount.Should().Be(initialUsageCount + 1);
        }

        [Fact]
        public void DecrementUsage_WithPositiveUsageCount_ShouldDecreaseUsageCount()
        {
            // Arrange
            var tag = new TagBuilder().WithUsageCount(5).Build();
            var initialUsageCount = tag.UsageCount;

            // Act
            tag.DecrementUsage();

            // Assert
            tag.UsageCount.Should().Be(initialUsageCount - 1);
        }

        [Fact]
        public void DecrementUsage_WithZeroUsageCount_ShouldThrowDomainException()
        {
            // Arrange
            var tag = new TagBuilder().WithUsageCount(0).Build();

            // Act & Assert
            var action = () => tag.DecrementUsage();
            action.Should().Throw<DomainException>()
                .WithMessage("Usage count cannot be negative.");
        }

        [Fact]
        public void UpdateDescription_WithValidDescription_ShouldUpdateDescription()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var newDescription = "This is an updated tag description.";

            // Act
            tag.UpdateDescription(newDescription);

            // Assert
            tag.Description.Should().Be(newDescription);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateDescription_WithInvalidDescription_ShouldThrowException(string description)
        {
            // Arrange
            var tag = new TagBuilder().Build();

            // Act & Assert
            var action = () => tag.UpdateDescription(description);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateDescription_WithDescriptionTooLong_ShouldThrowException()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var description = new string('a', 201); // Max is 200

            // Act & Assert
            var action = () => tag.UpdateDescription(description);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddQuestion_WithValidQuestion_ShouldAddQuestionAndIncrementUsage()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var question = new QuestionBuilder().Build();
            var initialUsageCount = tag.UsageCount;

            // Act
            tag.AddQuestion(question);

            // Assert
            tag.Questions.Should().Contain(question);
            tag.UsageCount.Should().Be(initialUsageCount + 1);
        }

        [Fact]
        public void AddQuestion_WithNullQuestion_ShouldThrowException()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            Question question = null;

            // Act & Assert
            var action = () => tag.AddQuestion(question);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddQuestion_WithDuplicateQuestion_ShouldThrowDomainException()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var question = new QuestionBuilder().Build();
            tag.AddQuestion(question);

            // Act & Assert
            var action = () => tag.AddQuestion(question);
            action.Should().Throw<DomainException>()
                .WithMessage("Question is already associated with this tag.");
        }

        [Fact]
        public void RemoveQuestion_WithExistingQuestion_ShouldRemoveQuestionAndDecrementUsage()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var question = new QuestionBuilder().Build();
            tag.AddQuestion(question);
            var usageCountAfterAdd = tag.UsageCount;

            // Act
            tag.RemoveQuestion(question);

            // Assert
            tag.Questions.Should().NotContain(question);
            tag.UsageCount.Should().Be(usageCountAfterAdd - 1);
        }

        [Fact]
        public void RemoveQuestion_WithNullQuestion_ShouldThrowException()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            Question question = null;

            // Act & Assert
            var action = () => tag.RemoveQuestion(question);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveQuestion_WithNonExistentQuestion_ShouldThrowDomainException()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act & Assert
            var action = () => tag.RemoveQuestion(question);
            action.Should().Throw<DomainException>()
                .WithMessage("Question is not associated with this tag.");
        }

        [Fact]
        public void UsageCount_WithMultipleQuestions_ShouldTrackCorrectly()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var question1 = new QuestionBuilder().WithTitle("Question 1").Build();
            var question2 = new QuestionBuilder().WithTitle("Question 2").Build();
            var question3 = new QuestionBuilder().WithTitle("Question 3").Build();

            // Act
            tag.AddQuestion(question1);
            tag.AddQuestion(question2);
            tag.AddQuestion(question3);

            // Assert
            tag.UsageCount.Should().Be(3);
            tag.Questions.Should().HaveCount(3);
        }

        [Fact]
        public void UsageCount_AddAndRemoveQuestions_ShouldTrackCorrectly()
        {
            // Arrange
            var tag = new TagBuilder().Build();
            var question1 = new QuestionBuilder().WithTitle("Question 1").Build();
            var question2 = new QuestionBuilder().WithTitle("Question 2").Build();

            // Act
            tag.AddQuestion(question1);
            tag.AddQuestion(question2);
            tag.RemoveQuestion(question1);

            // Assert
            tag.UsageCount.Should().Be(1);
            tag.Questions.Should().HaveCount(1);
            tag.Questions.Should().Contain(question2);
            tag.Questions.Should().NotContain(question1);
        }

        [Fact]
        public void CreatedAt_WhenTagCreated_ShouldBeSetToCurrentTime()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;
            var name = "test-tag";
            var description = "This is a test tag description.";

            // Act
            var tag = Tag.Create(name, description);

            // Assert
            tag.CreatedAt.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void ManualUsageIncrement_ShouldNotAffectQuestionCollection()
        {
            // Arrange
            var tag = new TagBuilder().Build();

            // Act
            tag.IncrementUsage();
            tag.IncrementUsage();

            // Assert
            tag.UsageCount.Should().Be(2);
            tag.Questions.Should().BeEmpty();
        }

        [Fact]
        public void UsageCount_WithBuilderUsageCount_ShouldSetCorrectly()
        {
            // Arrange & Act
            var tag = new TagBuilder().WithUsageCount(10).Build();

            // Assert
            tag.UsageCount.Should().Be(10);
        }
    }
}