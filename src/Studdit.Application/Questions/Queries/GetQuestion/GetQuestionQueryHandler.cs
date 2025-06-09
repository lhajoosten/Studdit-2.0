using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Questions.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Questions.Queries.GetQuestion
{
    public class GetQuestionQueryHandler : IQueryHandler<GetQuestionQuery, QuestionDto>
    {
        private readonly IQueryRepository<Question> _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetQuestionQueryHandler(IQueryRepository<Question> questionRepository, IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<QuestionDto>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
        {
            // Get the question with includes for related data
            var question = await _questionRepository.GetByIdAsync(
                request.Id,
                cancellationToken,
                q => q.Author,
                q => q.Tags,
                q => q.Answers);

            if (question == null)
            {
                return Result.Failure<QuestionDto>("Question not found.");
            }

            // Increment view count
            question.IncrementViewCount();
            var repository = _unitOfWork.CommandRepository<Question>();
            await repository.UpdateAsync(question, 0, cancellationToken); // System update for view count
            await _unitOfWork.SaveChangesAsync(0, cancellationToken);

            // Map to DTO
            var questionDto = await _questionRepository.GetByIdProjectedAsync<QuestionDto>(request.Id, cancellationToken);

            return Result.Success(questionDto!);
        }
    }
}
