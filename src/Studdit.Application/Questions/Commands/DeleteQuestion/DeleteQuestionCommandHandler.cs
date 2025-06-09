using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Questions.Commands.DeleteQuestion
{
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Question> _questionRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteQuestionCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Question> questionRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User must be authenticated.");
            }

            // Get the question
            var question = await _questionRepository.GetByIdAsync(request.Id, cancellationToken, q => q.Author);
            if (question == null)
            {
                return Result.Failure("Question not found.");
            }

            // Check authorization
            if (question.Author.Id != _currentUserService.UserId.Value)
            {
                return Result.Failure("You can only delete your own questions.");
            }

            try
            {
                var repository = _unitOfWork.CommandRepository<Question>();
                await repository.DeleteAsync(question, cancellationToken);
                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete question: {ex.Message}");
            }
        }
    }
}
