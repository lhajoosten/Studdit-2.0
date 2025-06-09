using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Answers.Commands.AcceptAnswer
{
    public class AcceptAnswerCommandHandler : ICommandHandler<AcceptAnswerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Answer> _answerRepository;
        private readonly ICurrentUserService _currentUserService;

        public AcceptAnswerCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Answer> answerRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _answerRepository = answerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User must be authenticated.");
            }

            // Get the answer with its question and author
            var answer = await _answerRepository.GetByIdAsync(request.AnswerId, cancellationToken, a => a.Question, a => a.Question.Author);
            if (answer == null)
            {
                return Result.Failure("Answer not found.");
            }

            // Check authorization - only question author can accept answers
            if (answer.Question.Author.Id != _currentUserService.UserId.Value)
            {
                return Result.Failure("Only the question author can accept answers.");
            }

            if (answer.Question.IsClosed)
            {
                return Result.Failure("Cannot accept an answer for a closed question.");
            }

            if (answer.IsAccepted)
            {
                return Result.Failure("Answer is already accepted.");
            }

            try
            {
                // First, unmark any previously accepted answers for this question
                var existingAcceptedAnswers = await _answerRepository.FindAsync(
                    a => a.Question.Id == answer.Question.Id && a.IsAccepted,
                    cancellationToken);

                var repository = _unitOfWork.CommandRepository<Answer>();

                foreach (var acceptedAnswer in existingAcceptedAnswers)
                {
                    acceptedAnswer.UnmarkAsAccepted();
                    await repository.UpdateAsync(acceptedAnswer, _currentUserService.UserId.Value, cancellationToken);
                }

                // Mark the new answer as accepted
                answer.MarkAsAccepted();
                await repository.UpdateAsync(answer, _currentUserService.UserId.Value, cancellationToken);

                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to accept answer: {ex.Message}");
            }
        }
    }
}