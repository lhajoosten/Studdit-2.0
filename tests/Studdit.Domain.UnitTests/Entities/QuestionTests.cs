using FluentAssertions;
using Studdit.Domain.Entities;
using Studdit.Domain.Enums;
using Studdit.Domain.Exceptions;
using Studdit.Test.Utils.Builders;

namespace Studdit.Domain.UnitTests.Entities
{
    public class QuestionTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateQuestion()
        {
            // Arrange
            var title = "Test Question Title";
            var content = "This is a test question content that meets the minimum length requirement.";
            var author = new UserBuilder().Build();

            // Act
            var question = Question.Create(title, content, author);

            // Assert
            question.Should().NotBeNull();
            question.Title.Should().Be(title);
            question.Content.Should().Be(content);
            question.Author.Should().Be(author);
            question.VoteScore.Should().Be(0);
            question.ViewCount.Should().Be(0);
            question.IsAnswered.Should().BeFalse();
            question.IsClosed.Should().BeFalse();
            question.ClosedAt.Should().BeNull();
            question.ClosureReason.Should().BeNull();
            question.Answers.Should().BeEmpty();
            question.Votes.Should().BeEmpty();
            question.Tags.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidTitle_ShouldThrowException(string title)
        {
            // Arrange
            var content = "This is a test question content that meets the minimum length requirement.";
            var author = new UserBuilder().Build();

            // Act & Assert
            var action = () => Question.Create(title, content, author);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithTitleTooLong_ShouldThrowException()
        {
            // Arrange
            var title = new string('a', 151); // Max is 150
            var content = "This is a test question content that meets the minimum length requirement.";
            var author = new UserBuilder().Build();

            // Act & Assert
            var action = () => Question.Create(title, content, author);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidContent_ShouldThrowException(string content)
        {
            // Arrange
            var title = "Test Question Title";
            var author = new UserBuilder().Build();

            // Act & Assert
            var action = () => Question.Create(title, content, author);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithContentTooShort_ShouldThrowException()
        {
            // Arrange
            var title = "Test Question Title";
            var content = "Short"; // Min is 30 characters
            var author = new UserBuilder().Build();

            // Act & Assert
            var action = () => Question.Create(title, content, author);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Create_WithNullAuthor_ShouldThrowException()
        {
            // Arrange
            var title = "Test Question Title";
            var content = "This is a test question content that meets the minimum length requirement.";
            User author = null;

            // Act & Assert
            var action = () => Question.Create(title, content, author);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateQuestion()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var newTitle = "Updated Question Title";
            var newContent = "This is the updated question content that meets the minimum length requirement.";

            // Act
            question.Update(newTitle, newContent);

            // Assert
            question.Title.Should().Be(newTitle);
            question.Content.Should().Be(newContent);
            question.LastModifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Update_WithClosedQuestion_ShouldThrowDomainException()
        {
            // Arrange
            var question = new QuestionBuilder().WithClosed().Build();
            var newTitle = "Updated Question Title";
            var newContent = "This is the updated question content that meets the minimum length requirement.";

            // Act & Assert
            var action = () => question.Update(newTitle, newContent);
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot update a closed question.");
        }

        [Fact]
        public void AddAnswer_WithValidAnswer_ShouldAddAnswerAndMarkAsAnswered()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var answer = new AnswerBuilder().WithQuestion(question).Build();

            // Act
            question.AddAnswer(answer);

            // Assert
            question.Answers.Should().Contain(answer);
            question.IsAnswered.Should().BeTrue();
        }

        [Fact]
        public void AddAnswer_WithNullAnswer_ShouldThrowException()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            Answer answer = null;

            // Act & Assert
            var action = () => question.AddAnswer(answer);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddAnswer_WithClosedQuestion_ShouldThrowDomainException()
        {
            // Arrange
            var question = new QuestionBuilder().WithClosed().Build();
            var answer = new AnswerBuilder().Build();

            // Act & Assert
            var action = () => question.AddAnswer(answer);
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot add an answer to a closed question.");
        }

        [Fact]
        public void AddVote_WithNewVote_ShouldAddVoteAndUpdateScore()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var user = new UserBuilder().WithUsername("voter").Build();
            var vote = new VoteBuilder().WithUser(user).WithType(VoteType.Upvote).ForQuestion(question).Build();

            // Act
            question.AddVote(vote);

            // Assert
            question.Votes.Should().Contain(vote);
            question.VoteScore.Should().Be(1);
        }

        [Fact]
        public void AddVote_WithExistingUserSameVoteType_ShouldThrowDomainException()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var user = new UserBuilder().WithUsername("voter").Build();
            var firstVote = new VoteBuilder().WithUser(user).WithType(VoteType.Upvote).ForQuestion(question).Build();
            var secondVote = new VoteBuilder().WithUser(user).WithType(VoteType.Upvote).ForQuestion(question).Build();

            question.AddVote(firstVote);

            // Act & Assert
            var action = () => question.AddVote(secondVote);
            action.Should().Throw<DomainException>()
                .WithMessage("User already voted with the same vote type.");
        }

        [Fact]
        public void AddVote_WithExistingUserDifferentVoteType_ShouldReplaceVote()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var user = new UserBuilder().WithUsername("voter").Build();
            var upvote = new VoteBuilder().WithUser(user).WithType(VoteType.Upvote).ForQuestion(question).Build();
            var downvote = new VoteBuilder().WithUser(user).WithType(VoteType.Downvote).ForQuestion(question).Build();

            question.AddVote(upvote);

            // Act
            question.AddVote(downvote);

            // Assert
            question.Votes.Should().HaveCount(1);
            question.Votes.Should().Contain(downvote);
            question.Votes.Should().NotContain(upvote);
            question.VoteScore.Should().Be(-1);
        }

        [Fact]
        public void AddTag_WithValidTag_ShouldAddTagAndIncrementUsage()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var tag = new TagBuilder().WithName("test-tag").Build();
            var initialUsageCount = tag.UsageCount;

            // Act
            question.AddTag(tag);

            // Assert
            question.Tags.Should().Contain(tag);
            tag.UsageCount.Should().Be(initialUsageCount + 1);
        }

        [Fact]
        public void AddTag_WithNullTag_ShouldThrowException()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            Tag tag = null;

            // Act & Assert
            var action = () => question.AddTag(tag);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddTag_WithMaxTagsReached_ShouldThrowDomainException()
        {
            // Arrange
            var question = new QuestionBuilder().Build();

            // Add 5 tags (max limit)
            for (int i = 0; i < 5; i++)
            {
                var tag = new TagBuilder().WithName($"tag-{i}").Build();
                question.AddTag(tag);
            }

            var extraTag = new TagBuilder().WithName("extra-tag").Build();

            // Act & Assert
            var action = () => question.AddTag(extraTag);
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot add more than 5 tags to a question.");
        }

        [Fact]
        public void AddTag_WithDuplicateTag_ShouldNotAddDuplicate()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var tag = new TagBuilder().WithName("duplicate-tag").Build();

            question.AddTag(tag);
            var initialUsageCount = tag.UsageCount;

            // Act
            question.AddTag(tag);

            // Assert
            question.Tags.Should().HaveCount(1);
            tag.UsageCount.Should().Be(initialUsageCount); // Should not increment again
        }

        [Fact]
        public void RemoveTag_WithExistingTag_ShouldRemoveTagAndDecrementUsage()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var tag = new TagBuilder().WithName("remove-tag").Build();
            question.AddTag(tag);
            var usageCountAfterAdd = tag.UsageCount;

            // Act
            question.RemoveTag(tag);

            // Assert
            question.Tags.Should().NotContain(tag);
            tag.UsageCount.Should().Be(usageCountAfterAdd - 1);
        }

        [Fact]
        public void RemoveTag_WithNonExistentTag_ShouldThrowDomainException()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var tag = new TagBuilder().WithName("non-existent-tag").Build();

            // Act & Assert
            var action = () => question.RemoveTag(tag);
            action.Should().Throw<DomainException>()
                .WithMessage("Tag is not associated with this question.");
        }

        [Fact]
        public void IncrementViewCount_ShouldIncreaseViewCount()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var initialViewCount = question.ViewCount;

            // Act
            question.IncrementViewCount();

            // Assert
            question.ViewCount.Should().Be(initialViewCount + 1);
        }

        [Fact]
        public void Close_WithValidReason_ShouldCloseQuestion()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var reason = "Duplicate question";

            // Act
            question.Close(reason);

            // Assert
            question.IsClosed.Should().BeTrue();
            question.ClosureReason.Should().Be(reason);
            question.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Close_WithInvalidReason_ShouldThrowException(string reason)
        {
            // Arrange
            var question = new QuestionBuilder().Build();

            // Act & Assert
            var action = () => question.Close(reason);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Close_WithAlreadyClosedQuestion_ShouldThrowDomainException()
        {
            // Arrange
            var question = new QuestionBuilder().WithClosed().Build();
            var reason = "Another reason";

            // Act & Assert
            var action = () => question.Close(reason);
            action.Should().Throw<DomainException>()
                .WithMessage("Question is already closed.");
        }

        [Fact]
        public void Reopen_WithClosedQuestion_ShouldReopenQuestion()
        {
            // Arrange
            var question = new QuestionBuilder().WithClosed().Build();

            // Act
            question.Reopen();

            // Assert
            question.IsClosed.Should().BeFalse();
            question.ClosedAt.Should().BeNull();
            question.ClosureReason.Should().BeNull();
        }

        [Fact]
        public void Reopen_WithOpenQuestion_ShouldThrowDomainException()
        {
            // Arrange
            var question = new QuestionBuilder().Build();

            // Act & Assert
            var action = () => question.Reopen();
            action.Should().Throw<DomainException>()
                .WithMessage("Question is not closed.");
        }

        [Fact]
        public void VoteScore_WithMultipleVotes_ShouldCalculateCorrectly()
        {
            // Arrange
            var question = new QuestionBuilder().Build();
            var user1 = new UserBuilder().WithUsername("user1").Build();
            var user2 = new UserBuilder().WithUsername("user2").Build();
            var user3 = new UserBuilder().WithUsername("user3").Build();

            var upvote1 = new VoteBuilder().WithUser(user1).WithType(VoteType.Upvote).ForQuestion(question).Build();
            var upvote2 = new VoteBuilder().WithUser(user2).WithType(VoteType.Upvote).ForQuestion(question).Build();
            var downvote = new VoteBuilder().WithUser(user3).WithType(VoteType.Downvote).ForQuestion(question).Build();

            // Act
            question.AddVote(upvote1);
            question.AddVote(upvote2);
            question.AddVote(downvote);

            // Assert
            question.VoteScore.Should().Be(1); // 2 upvotes - 1 downvote = 1
        }
    }
}