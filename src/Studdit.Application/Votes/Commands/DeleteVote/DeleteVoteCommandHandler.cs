using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Votes.Commands.DeleteVote
{
    public class DeleteVoteCommandHandler : ICommandHandler<DeleteVoteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Vote> _voteRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteVoteCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Vote> voteRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _voteRepository = voteRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteVoteCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User must be authenticated.");
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
                return Result.Failure("You can only delete your own votes.");
            }

            try
            {
                // Update vote scores in the target entity before deleting
                if (vote.Question != null)
                {
                    var questionRepository = _unitOfWork.CommandRepository<Question>();
                    await questionRepository.UpdateAsync(vote.Question, _currentUserService.UserId.Value, cancellationToken);
                }
                else if (vote.Answer != null)
                {
                    var answerRepository = _unitOfWork.CommandRepository<Answer>();
                    await answerRepository.UpdateAsync(vote.Answer, _currentUserService.UserId.Value, cancellationToken);
                }

                // Delete the vote
                var voteRepository = _unitOfWork.CommandRepository<Vote>();
                await voteRepository.DeleteAsync(vote, cancellationToken);

                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete vote: {ex.Message}");
            }
        }
    }
}