using Studdit.Application.Answers.Models;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Common.Specifications;
using Studdit.Domain.Entities;

namespace Studdit.Application.Answers.Queries.GetAnswers
{
    public class GetAnswersQueryHandler : IQueryHandler<GetAnswersQuery, PaginatedList<AnswerSummaryDto>>
    {
        private readonly IQueryRepository<Answer> _answerRepository;

        public GetAnswersQueryHandler(IQueryRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<Result<PaginatedList<AnswerSummaryDto>>> Handle(GetAnswersQuery request, CancellationToken cancellationToken)
        {
            var specification = new BaseEntitySpecification<Answer>();

            // Apply filters
            if (request.QuestionId.HasValue)
            {
                specification = specification.Where(a => a.Question.Id == request.QuestionId.Value);
            }

            if (request.AuthorId.HasValue)
            {
                specification = specification.Where(a => a.Author.Id == request.AuthorId.Value);
            }

            if (request.IsAccepted.HasValue)
            {
                specification = specification.Where(a => a.IsAccepted == request.IsAccepted.Value);
            }

            // Apply sorting
            specification = request.SortBy?.ToLowerInvariant() switch
            {
                "votescore" => request.SortDescending
                    ? specification.OrderByDescending(a => a.VoteScore)
                    : specification.OrderBy(a => a.VoteScore),
                "isaccepted" => request.SortDescending
                    ? specification.OrderByDescending(a => a.IsAccepted)
                    : specification.OrderBy(a => a.IsAccepted),
                _ => request.SortDescending
                    ? specification.OrderByDescending(a => a.CreatedDate)
                    : specification.OrderBy(a => a.CreatedDate)
            };

            // Apply pagination
            specification = specification.Paginate(request.PageNumber, request.PageSize);

            // Include related data
            specification = specification.Include(a => a.Author).Include(a => a.Question);

            var result = await _answerRepository.GetPaginatedProjectedAsync<AnswerSummaryDto>(
                specification,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return Result.Success(result);
        }
    }
}