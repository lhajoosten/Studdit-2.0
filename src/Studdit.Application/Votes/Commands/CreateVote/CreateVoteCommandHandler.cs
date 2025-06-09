using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;
using Studdit.Domain.Enums;

namespace Studdit.Application.Votes.Commands.CreateVote
{
    public class CreateVoteCommandHandler : ICommandHandler<CreateVoteCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Question> _questionRepository;
        private readonly IQueryRepository<Answer> _answerRepository;
        private readonly IQueryRepository<User> _userRepository;
        private readonly IQueryRepository<Vote> _voteRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateVoteCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Question> questionRepository,
            IQueryRepository<Answer> answerRepository,
            IQueryRepository<User> userRepository,
            IQueryRepository<Vote> voteRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _voteRepository = voteRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<int>> Handle(CreateVoteCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure<int>("User must be authenticated.");
            }

            // Get the current user
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
            if (user == null || !user.IsActive)
            {
                return Result.Failure<int>("User not found or inactive.");
            }

            // Parse vote type
            var voteType = request.VoteType == "Upvote" ? VoteType.Upvote : VoteType.Downvote;

            // Check user permissions
            if (voteType == VoteType.Upvote && !user.CanUpvote())
            {
                return Result.Failure<int>($"You need {15 - user.Reputation} more reputation points to upvote.");
            }

            if (voteType == VoteType.Downvote && !user.CanDownvote())
            {
                return Result.Failure<int>($"You need {125 - user.Reputation} more reputation points to downvote.");
            }

            Question? question = null;
            Answer? answer = null;

            // Validate target exists and check for existing vote
            if (request.QuestionId.HasValue)
            {
                question = await _questionRepository.GetByIdAsync(request.QuestionId.Value, cancellationToken, q => q.Author);
                if (question == null)
                {
                    return Result.Failure<int>("Question not found.");
                }

                // Check if user is trying to vote on their own question
                if (question.Author.Id == user.Id)
                {
                    return Result.Failure<int>("You cannot vote on your own question.");
                }

                // Check for existing vote
                var existingVote = await _voteRepository.FirstOrDefaultAsync(
                    v => v.User.Id == user.Id && v.Question.Id == question.Id, cancellationToken);

                if (existingVote != null)
                {
                    return Result.Failure<int>("You have already voted on this question.");
                }
            }
            else if (request.AnswerId.HasValue)
            {
                answer = await _answerRepository.GetByIdAsync(request.AnswerId.Value, cancellationToken, a => a.Author);
                if (answer == null)
                {
                    return Result.Failure<int>("Answer not found.");
                }

                // Check if user is trying to vote on their own answer
                if (answer.Author.Id == user.Id)
                {
                    return Result.Failure<int>("You cannot vote on your own answer.");
                }

                // Check for existing vote
                var existingVote = await _voteRepository.FirstOrDefaultAsync(
                    v => v.User.Id == user.Id && v.Answer.Id == answer.Id, cancellationToken);

                if (existingVote != null)
                {
                    return Result.Failure<int>("You have already voted on this answer.");
                }
            }

            try
            {
                // Create the vote
                Vote vote;
                if (question != null)
                {
                    vote = Vote.CreateForQuestion(voteType, user, question);
                    question.AddVote(vote);
                }
                else
                {
                    vote = Vote.CreateForAnswer(voteType, user, answer!);
                    answer!.AddVote(vote);
                }

                // Save the vote
                var voteRepository = _unitOfWork.CommandRepository<Vote>();
                var createdVote = await voteRepository.AddAsync(vote, _currentUserService.UserId.Value, cancellationToken);

                // Update the target entity
                if (question != null)
                {
                    var questionRepository = _unitOfWork.CommandRepository<Question>();
                    await questionRepository.UpdateAsync(question, _currentUserService.UserId.Value, cancellationToken);
                }
                else
                {
                    var answerRepository = _unitOfWork.CommandRepository<Answer>();
                    await answerRepository.UpdateAsync(answer!, _currentUserService.UserId.Value, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success(createdVote.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>($"Failed to create vote: {ex.Message}");
            }
        }
    }
}