using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Answers.Commands.DeleteAnswer
{
    public class DeleteAnswerCommandHandler : ICommandHandler<DeleteAnswerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Answer> _answerRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteAnswerCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Answer> answerRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _answerRepository = answerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User must be authenticated.");
            }

            // Get the answer with its author
            var answer = await _answerRepository.GetByIdAsync(request.Id, cancellationToken, a => a.Author, a => a.Question);
            if (answer == null)
            {
                return Result.Failure("Answer not found.");
            }

            // Check authorization
            if (answer.Author.Id != _currentUserService.UserId.Value)
            {
                return Result.Failure("You can only delete your own answers.");
            }

            if (answer.Question.IsClosed)
            {
                return Result.Failure("Cannot delete an answer for a closed question.");
            }

            try
            {
                var repository = _unitOfWork.CommandRepository<Answer>();
                await repository.DeleteAsync(answer, cancellationToken);
                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete answer: {ex.Message}");
            }
        }
    }
}