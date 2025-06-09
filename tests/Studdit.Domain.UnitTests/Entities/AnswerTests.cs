using FluentAssertions;
using Studdit.Domain.Entities;
using Studdit.Domain.Enums;
using Studdit.Domain.Exceptions;
using Studdit.Test.Utils.Builders;

namespace Studdit.Domain.UnitTests.Entities
{
    public class AnswerTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateAnswer()
        {
            // Arrange
            var content = "This is a test answer content.";
            var author = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var answer = Answer.Create(content, author, question);

            // Assert
            answer.Should().NotBeNull();
            answer.Content.Should().Be(content);
            answer.Author.Should().Be(author);
            answer.Question.Should().Be(question);
            answer.VoteScore.Should().Be(0);
            answer.IsAccepted.Should().BeFalse();
            answer.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            answer.LastModifiedDate.Should().BeNull();
            answer.Votes.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidContent_ShouldThrowException(string content)
        {
            // Arrange
            var author = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act & Assert
            var action = () => Answer.Create(content, author, question);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullAuthor_ShouldThrowException()
        {
            // Arrange
            var content = "This is a test answer content.";
            var question = new QuestionBuilder().Build();

            // Act & Assert
            var action = () => Answer.Create(content, null!, question);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullQuestion_ShouldThrowException()
        {
            // Arrange
            var content = "This is a test answer content.";
            var author = new UserBuilder().Build();

            // Act & Assert
            var action = () => Answer.Create(content, author, null!);
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Create_WithClosedQuestion_ShouldThrowDomainException()
        {
            // Arrange
            var content = "This is a test answer content.";
            var author = new UserBuilder().Build();
            var question = new QuestionBuilder().WithClosed().Build();

            // Act & Assert
            var action = () => Answer.Create(content, author, question);
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot create an answer for a closed question.");
        }

        [Fact]
        public void Update_WithValidContent_ShouldUpdateAnswer()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            var newContent = "This is updated answer content.";

            // Act
            answer.Update(newContent);

            // Assert
            answer.Content.Should().Be(newContent);
            answer.LastModifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Update_WithInvalidContent_ShouldThrowException(string content)
        {
            // Arrange
            var answer = new AnswerBuilder().Build();

            // Act & Assert
            var action = () => answer.Update(content);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MarkAsAccepted_WithQuestionAuthor_ShouldMarkAsAccepted()
        {
            // Arrange
            var questionAuthor = new UserBuilder().WithUsername("questionauthor").Build();
            var question = new QuestionBuilder().WithAuthor(questionAuthor).Build();
            var answerAuthor = new UserBuilder().WithUsername("answerauthor").Build();
            var answer = new AnswerBuilder().WithQuestion(question).WithAuthor(questionAuthor).Build();

            // Act
            answer.MarkAsAccepted();

            // Assert
            answer.IsAccepted.Should().BeTrue();
        }

        [Fact]
        public void MarkAsAccepted_WithNonQuestionAuthor_ShouldThrowDomainException()
        {
            // Arrange
            var questionAuthor = new UserBuilder().WithUsername("questionauthor").Build();
            var question = new QuestionBuilder().WithAuthor(questionAuthor).Build();
            var answerAuthor = new UserBuilder().WithUsername("answerauthor").Build();
            var answer = new AnswerBuilder().WithQuestion(question).WithAuthor(answerAuthor).Build();

            // Act & Assert
            var action = () => answer.MarkAsAccepted();
            action.Should().Throw<DomainException>()
                .WithMessage("Only the question author can mark an answer as accepted.");
        }

        [Fact]
        public void UnmarkAsAccepted_WithAcceptedAnswer_ShouldUnmarkAsAccepted()
        {
            // Arrange
            var questionAuthor = new UserBuilder().WithUsername("questionauthor").Build();
            var question = new QuestionBuilder().WithAuthor(questionAuthor).Build();
            var answerAuthor = new UserBuilder().WithUsername("answerauthor").Build();
            var answer = new AnswerBuilder().WithQuestion(question).WithAuthor(questionAuthor).WithAccepted().Build();

            // Act
            answer.UnmarkAsAccepted();

            // Assert
            answer.IsAccepted.Should().BeFalse();
        }

        [Fact]
        public void UnmarkAsAccepted_WithNonAcceptedAnswer_ShouldThrowDomainException()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();

            // Act & Assert
            var action = () => answer.UnmarkAsAccepted();
            action.Should().Throw<DomainException>()
                .WithMessage("Answer is not marked as accepted.");
        }

        [Fact]
        public void AddVote_WithValidVote_ShouldAddVoteAndUpdateScore()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            var voter = new UserBuilder().WithUsername("voter").Build();
            var vote = new VoteBuilder().WithUser(voter).WithType(VoteType.Upvote).ForAnswer(answer).Build();

            // Act
            answer.AddVote(vote);

            // Assert
            answer.Votes.Should().Contain(vote);
            answer.VoteScore.Should().Be(1);
        }

        [Fact]
        public void AddVote_WithNullVote_ShouldThrowException()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            Vote vote = null;

            // Act & Assert
            var action = () => answer.AddVote(vote);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddVote_WithMultipleVotes_ShouldCalculateScoreCorrectly()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            var voter1 = new UserBuilder().WithUsername("voter1").Build();
            var voter2 = new UserBuilder().WithUsername("voter2").Build();
            var voter3 = new UserBuilder().WithUsername("voter3").Build();

            var upvote1 = new VoteBuilder().WithUser(voter1).WithType(VoteType.Upvote).ForAnswer(answer).Build();
            var upvote2 = new VoteBuilder().WithUser(voter2).WithType(VoteType.Upvote).ForAnswer(answer).Build();
            var downvote = new VoteBuilder().WithUser(voter3).WithType(VoteType.Downvote).ForAnswer(answer).Build();

            // Act
            answer.AddVote(upvote1);
            answer.AddVote(upvote2);
            answer.AddVote(downvote);

            // Assert
            answer.VoteScore.Should().Be(1); // 2 upvotes - 1 downvote = 1
            answer.Votes.Should().HaveCount(3);
        }

        [Fact]
        public void VoteScore_WithOnlyUpvotes_ShouldBePositive()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            var voter1 = new UserBuilder().WithUsername("voter1").Build();
            var voter2 = new UserBuilder().WithUsername("voter2").Build();

            var upvote1 = new VoteBuilder().WithUser(voter1).WithType(VoteType.Upvote).ForAnswer(answer).Build();
            var upvote2 = new VoteBuilder().WithUser(voter2).WithType(VoteType.Upvote).ForAnswer(answer).Build();

            // Act
            answer.AddVote(upvote1);
            answer.AddVote(upvote2);

            // Assert
            answer.VoteScore.Should().Be(2);
        }

        [Fact]
        public void VoteScore_WithOnlyDownvotes_ShouldBeNegative()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            var voter1 = new UserBuilder().WithUsername("voter1").Build();
            var voter2 = new UserBuilder().WithUsername("voter2").Build();

            var downvote1 = new VoteBuilder().WithUser(voter1).WithType(VoteType.Downvote).ForAnswer(answer).Build();
            var downvote2 = new VoteBuilder().WithUser(voter2).WithType(VoteType.Downvote).ForAnswer(answer).Build();

            // Act
            answer.AddVote(downvote1);
            answer.AddVote(downvote2);

            // Assert
            answer.VoteScore.Should().Be(-2);
        }

        [Fact]
        public void VoteScore_WithEqualUpvotesAndDownvotes_ShouldBeZero()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            var voter1 = new UserBuilder().WithUsername("voter1").Build();
            var voter2 = new UserBuilder().WithUsername("voter2").Build();

            var upvote = new VoteBuilder().WithUser(voter1).WithType(VoteType.Upvote).ForAnswer(answer).Build();
            var downvote = new VoteBuilder().WithUser(voter2).WithType(VoteType.Downvote).ForAnswer(answer).Build();

            // Act
            answer.AddVote(upvote);
            answer.AddVote(downvote);

            // Assert
            answer.VoteScore.Should().Be(0);
        }

        [Fact]
        public void CreatedAt_WhenAnswerCreated_ShouldBeSetToCurrentTime()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;
            var content = "This is a test answer content.";
            var author = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var answer = Answer.Create(content, author, question);

            // Assert
            answer.CreatedDate.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void LastModifiedDate_WhenAnswerUpdated_ShouldBeSetToCurrentTime()
        {
            // Arrange
            var answer = new AnswerBuilder().Build();
            var beforeUpdate = DateTime.UtcNow;
            var newContent = "This is updated answer content.";

            // Act
            answer.Update(newContent);

            // Assert
            answer.LastModifiedDate.Should().NotBeNull();
            answer.LastModifiedDate.Value.Should().BeCloseTo(beforeUpdate, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void LastModifiedDate_WhenAnswerNotUpdated_ShouldBeNull()
        {
            // Arrange & Act
            var answer = new AnswerBuilder().Build();

            // Assert
            answer.LastModifiedDate.Should().BeNull();
        }
    }
}