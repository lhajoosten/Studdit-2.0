using FluentAssertions;
using Studdit.Domain.Common;
using Studdit.Domain.Entities;
using Studdit.Domain.Enums;
using Studdit.Domain.Exceptions;
using Studdit.Test.Utils.Builders;

namespace Studdit.Domain.UnitTests.Entities
{
    public class VoteTests
    {
        [Fact]
        public void CreateForQuestion_WithValidData_ShouldCreateVoteForQuestion()
        {
            // Arrange
            var type = VoteType.Upvote;
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var vote = Vote.CreateForQuestion(type, user, question);

            // Assert
            vote.Should().NotBeNull();
            vote.Type.Should().Be(type);
            vote.User.Should().Be(user);
            vote.Question.Should().Be(question);
            vote.Answer.Should().BeNull();
            vote.LastModifiedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            vote.LastModifiedByUserId.Should().Be(user.Id);
        }

        [Fact]
        public void CreateForQuestion_WithNullUser_ShouldThrowException()
        {
            // Arrange
            var type = VoteType.Upvote;
            User user = null;
            var question = new QuestionBuilder().Build();

            // Act & Assert
            var action = () => Vote.CreateForQuestion(type, user, question);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForQuestion_WithNullQuestion_ShouldThrowException()
        {
            // Arrange
            var type = VoteType.Upvote;
            var user = new UserBuilder().Build();
            Question question = null;

            // Act & Assert
            var action = () => Vote.CreateForQuestion(type, user, question);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForAnswer_WithValidData_ShouldCreateVoteForAnswer()
        {
            // Arrange
            var type = VoteType.Downvote;
            var user = new UserBuilder().Build();
            var answer = new AnswerBuilder().Build();

            // Act
            var vote = Vote.CreateForAnswer(type, user, answer);

            // Assert
            vote.Should().NotBeNull();
            vote.Type.Should().Be(type);
            vote.User.Should().Be(user);
            vote.Answer.Should().Be(answer);
            vote.Question.Should().BeNull();
            vote.LastModifiedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            vote.LastModifiedByUserId.Should().Be(user.Id);
        }

        [Fact]
        public void CreateForAnswer_WithNullUser_ShouldThrowException()
        {
            // Arrange
            var type = VoteType.Downvote;
            User user = null;
            var answer = new AnswerBuilder().Build();

            // Act & Assert
            var action = () => Vote.CreateForAnswer(type, user, answer);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForAnswer_WithNullAnswer_ShouldThrowException()
        {
            // Arrange
            var type = VoteType.Downvote;
            var user = new UserBuilder().Build();
            Answer answer = null;

            // Act & Assert
            var action = () => Vote.CreateForAnswer(type, user, answer);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeVoteType_WithValidData_ShouldChangeVoteType()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();
            var vote = Vote.CreateForQuestion(VoteType.Upvote, user, question);
            var newType = VoteType.Downvote;
            var beforeChange = DateTime.Now;

            // Act
            vote.ChangeVoteType(user, newType);

            // Assert
            vote.Type.Should().Be(newType);
            vote.LastModifiedDate.Should().BeCloseTo(beforeChange, TimeSpan.FromSeconds(1));
            vote.LastModifiedByUserId.Should().Be(user.Id);
        }

        [Fact]
        public void ChangeVoteType_WithNullUser_ShouldThrowException()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();
            var vote = Vote.CreateForQuestion(VoteType.Upvote, user, question);
            User changeUser = null;
            var newType = VoteType.Downvote;

            // Act & Assert
            var action = () => vote.ChangeVoteType(changeUser, newType);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangeVoteType_WithSameVoteType_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();
            var vote = Vote.CreateForQuestion(VoteType.Upvote, user, question);
            var sameType = VoteType.Upvote;

            // Act & Assert
            var action = () => vote.ChangeVoteType(user, sameType);
            action.Should().Throw<DomainException>()
                .WithMessage("Cannot change vote type to the same type.");
        }

        [Fact]
        public void ChangeVoteType_FromUpvoteToDownvote_ShouldUpdateCorrectly()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();
            var vote = Vote.CreateForQuestion(VoteType.Upvote, user, question);

            // Act
            vote.ChangeVoteType(user, VoteType.Downvote);

            // Assert
            vote.Type.Should().Be(VoteType.Downvote);
        }

        [Fact]
        public void ChangeVoteType_FromDownvoteToUpvote_ShouldUpdateCorrectly()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var answer = new AnswerBuilder().Build();
            var vote = Vote.CreateForAnswer(VoteType.Downvote, user, answer);

            // Act
            vote.ChangeVoteType(user, VoteType.Upvote);

            // Assert
            vote.Type.Should().Be(VoteType.Upvote);
        }

        [Fact]
        public void CreateForQuestion_WithUpvoteType_ShouldSetCorrectType()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var vote = Vote.CreateForQuestion(VoteType.Upvote, user, question);

            // Assert
            vote.Type.Should().Be(VoteType.Upvote);
        }

        [Fact]
        public void CreateForQuestion_WithDownvoteType_ShouldSetCorrectType()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var vote = Vote.CreateForQuestion(VoteType.Downvote, user, question);

            // Assert
            vote.Type.Should().Be(VoteType.Downvote);
        }

        [Fact]
        public void CreateForAnswer_WithUpvoteType_ShouldSetCorrectType()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var answer = new AnswerBuilder().Build();

            // Act
            var vote = Vote.CreateForAnswer(VoteType.Upvote, user, answer);

            // Assert
            vote.Type.Should().Be(VoteType.Upvote);
        }

        [Fact]
        public void CreateForAnswer_WithDownvoteType_ShouldSetCorrectType()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var answer = new AnswerBuilder().Build();

            // Act
            var vote = Vote.CreateForAnswer(VoteType.Downvote, user, answer);

            // Assert
            vote.Type.Should().Be(VoteType.Downvote);
        }

        [Fact]
        public void Vote_ShouldNotBeAssociatedWithBothQuestionAndAnswer()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();
            var answer = new AnswerBuilder().Build();

            // Act
            var questionVote = Vote.CreateForQuestion(VoteType.Upvote, user, question);
            var answerVote = Vote.CreateForAnswer(VoteType.Downvote, user, answer);

            // Assert
            questionVote.Question.Should().NotBeNull();
            questionVote.Answer.Should().BeNull();

            answerVote.Answer.Should().NotBeNull();
            answerVote.Question.Should().BeNull();
        }

        [Fact]
        public void LastModifiedDate_WhenVoteCreated_ShouldBeSetToCurrentTime()
        {
            // Arrange
            var beforeCreation = DateTime.Now;
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var vote = Vote.CreateForQuestion(VoteType.Upvote, user, question);

            // Assert
            vote.LastModifiedDate.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void LastModifiedByUserId_WhenVoteCreated_ShouldBeSetToUserId()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var vote = Vote.CreateForQuestion(VoteType.Upvote, user, question);

            // Assert
            vote.LastModifiedByUserId.Should().Be(user.Id);
        }

        [Fact]
        public void ChangeVoteType_WithDifferentUser_ShouldUpdateLastModifiedByUserId()
        {
            // Arrange
            var originalUser = new UserBuilder().WithUsername("original").Build();
            var modifyingUser = new UserBuilder().WithUsername("modifier").Build();
            var question = new QuestionBuilder().Build();
            var vote = Vote.CreateForQuestion(VoteType.Upvote, originalUser, question);

            // Act
            vote.ChangeVoteType(modifyingUser, VoteType.Downvote);

            // Assert
            vote.LastModifiedByUserId.Should().Be(modifyingUser.Id);
        }

        [Theory]
        [InlineData("Upvote")]
        [InlineData("Downvote")]
        public void CreateForQuestion_WithDifferentVoteTypes_ShouldCreateCorrectly(string voteType)
        {
            // Arrange
            var user = new UserBuilder().Build();
            var question = new QuestionBuilder().Build();

            // Act
            var vote = Vote.CreateForQuestion(Enumeration.FromName<VoteType>(voteType), user, question);

            // Assert
            vote.Type.Should().Be(Enumeration.FromName<VoteType>(voteType));
            vote.Question.Should().Be(question);
            vote.Answer.Should().BeNull();
        }

        [Theory]
        [InlineData("Upvote")]
        [InlineData("Downvote")]
        public void CreateForAnswer_WithDifferentVoteTypes_ShouldCreateCorrectly(string voteType)
        {
            // Arrange
            var user = new UserBuilder().Build();
            var answer = new AnswerBuilder().Build();

            // Act
            var vote = Vote.CreateForAnswer(Enumeration.FromName<VoteType>(voteType), user, answer);

            // Assert
            vote.Type.Should().Be(Enumeration.FromName<VoteType>(voteType));
            vote.Answer.Should().Be(answer);
            vote.Question.Should().BeNull();
        }
    }
}