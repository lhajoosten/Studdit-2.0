using Studdit.Application.Answers.Models;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Answers.Queries.GetAnswer
{
    public class GetAnswerQueryHandler : IQueryHandler<GetAnswerQuery, AnswerDto>
    {
        private readonly IQueryRepository<Answer> _answerRepository;

        public GetAnswerQueryHandler(IQueryRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<Result<AnswerDto>> Handle(GetAnswerQuery request, CancellationToken cancellationToken)
        {
            var answer = await _answerRepository.GetByIdProjectedAsync<AnswerDto>(request.Id, cancellationToken);

            if (answer == null)
            {
                return Result.Failure<AnswerDto>("Answer not found.");
            }

            return Result.Success(answer);
        }
    }
}