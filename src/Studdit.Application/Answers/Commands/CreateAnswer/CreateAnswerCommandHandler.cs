using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Answers.Commands.CreateAnswer
{
    public class CreateAnswerCommandHandler : ICommandHandler<CreateAnswerCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Question> _questionRepository;
        private readonly IQueryRepository<User> _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateAnswerCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Question> questionRepository,
            IQueryRepository<User> userRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<int>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure<int>("User must be authenticated.");
            }

            // Get the current user
            var author = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
            if (author == null || !author.IsActive)
            {
                return Result.Failure<int>("User not found or inactive.");
            }

            // Get the question
            var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
            if (question == null)
            {
                return Result.Failure<int>("Question not found.");
            }

            if (question.IsClosed)
            {
                return Result.Failure<int>("Cannot answer a closed question.");
            }

            try
            {
                // Create the answer
                var answer = Answer.Create(request.Content, author, question);

                // Save the answer
                var answerRepository = _unitOfWork.CommandRepository<Answer>();
                var createdAnswer = await answerRepository.AddAsync(answer, _currentUserService.UserId.Value, cancellationToken);

                // Update the question to mark as answered
                question.AddAnswer(answer);
                var questionRepository = _unitOfWork.CommandRepository<Question>();
                await questionRepository.UpdateAsync(question, _currentUserService.UserId.Value, cancellationToken);

                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success(createdAnswer.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>($"Failed to create answer: {ex.Message}");
            }
        }
    }
}