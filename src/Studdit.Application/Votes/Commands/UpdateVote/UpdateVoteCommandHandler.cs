using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;
using Studdit.Domain.Enums;

namespace Studdit.Application.Votes.Commands.UpdateVote
{
    public class UpdateVoteCommandHandler : ICommandHandler<UpdateVoteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Vote> _voteRepository;
        private readonly IQueryRepository<User> _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateVoteCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Vote> voteRepository,
            IQueryRepository<User> userRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateVoteCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User must be authenticated.");
            }

            // Get the current user
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
            if (user == null || !user.IsActive)
            {
                return Result.Failure("User not found or inactive.");
            }

            // Get the vote with its user, question, and answer
            var vote = await _voteRepository.GetByIdAsync(request.Id, cancellationToken, v => v.User, v => v.Question, v => v.Answer);
            if (vote == null)
            {
                return Result.Failure("Vote not found.");
            }

            // Check authorization
            if (vote.User.Id != _currentUserService.UserId.Value)
            {
                return Result.Failure("You can only modify your own votes.");
            }

            // Parse new vote type
            var newVoteType = request.VoteType == "Upvote" ? VoteType.Upvote : VoteType.Downvote;

            // Check if the vote type is actually changing
            if (vote.Type.Name == newVoteType.Name)
            {
                return Result.Failure("Vote is already of the specified type.");
            }

            // Check user permissions for the new vote type
            if (newVoteType == VoteType.Downvote && !user.CanDownvote())
            {
                return Result.Failure($"You need {125 - user.Reputation} more reputation points to downvote.");
            }

            try
            {
                // Change the vote type
                vote.ChangeVoteType(user, newVoteType);

                // Update vote scores in the target entity
                if (vote.Question != null)
                {
                    // Recalculate question vote score
                    var questionRepository = _unitOfWork.CommandRepository<Question>();
                    await questionRepository.UpdateAsync(vote.Question, _currentUserService.UserId.Value, cancellationToken);
                }
                else if (vote.Answer != null)
                {
                    // Recalculate answer vote score
                    var answerRepository = _unitOfWork.CommandRepository<Answer>();
                    await answerRepository.UpdateAsync(vote.Answer, _currentUserService.UserId.Value, cancellationToken);
                }

                // Save the vote
                var voteRepository = _unitOfWork.CommandRepository<Vote>();
                await voteRepository.UpdateAsync(vote, _currentUserService.UserId.Value, cancellationToken);

                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update vote: {ex.Message}");
            }
        }
    }
}