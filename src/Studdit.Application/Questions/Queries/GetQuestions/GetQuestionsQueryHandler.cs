using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Common.Specifications;
using Studdit.Application.Questions.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Questions.Queries.GetQuestions
{
    public class GetQuestionsQueryHandler : IQueryHandler<GetQuestionsQuery, PaginatedList<QuestionSummaryDto>>
    {
        private readonly IQueryRepository<Question> _questionRepository;

        public GetQuestionsQueryHandler(IQueryRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<Result<PaginatedList<QuestionSummaryDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
        {
            var specification = new BaseEntitySpecification<Question>();

            // Apply filters
            if (request.IsAnswered.HasValue)
            {
                specification = specification.Where(q => q.IsAnswered == request.IsAnswered.Value);
            }

            if (request.IsClosed.HasValue)
            {
                specification = specification.Where(q => q.IsClosed == request.IsClosed.Value);
            }

            if (request.AuthorId.HasValue)
            {
                specification = specification.Where(q => q.Author.Id == request.AuthorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                specification = specification.Where(q =>
                    q.Title.ToLower().Contains(searchTerm) ||
                    q.Content.ToLower().Contains(searchTerm));
            }

            if (request.TagNames?.Any() == true)
            {
                var normalizedTagNames = request.TagNames.Select(t => t.ToLowerInvariant()).ToList();
                specification = specification.Where(q => q.Tags.Any(t => normalizedTagNames.Contains(t.Name)));
            }

            // Apply sorting
            specification = request.SortBy?.ToLowerInvariant() switch
            {
                "votescore" => request.SortDescending
                    ? specification.OrderByDescending(q => q.VoteScore)
                    : specification.OrderBy(q => q.VoteScore),
                "viewcount" => request.SortDescending
                    ? specification.OrderByDescending(q => q.ViewCount)
                    : specification.OrderBy(q => q.ViewCount),
                "title" => request.SortDescending
                    ? specification.OrderByDescending(q => q.Title)
                    : specification.OrderBy(q => q.Title),
                _ => request.SortDescending
                    ? specification.OrderByDescending(q => q.CreatedDate)
                    : specification.OrderBy(q => q.CreatedDate)
            };

            // Apply pagination
            specification = specification.Paginate(request.PageNumber, request.PageSize);

            // Include related data
            specification = specification.Include(q => q.Author).Include(q => q.Tags).Include(q => q.Answers);

            var result = await _questionRepository.GetPaginatedProjectedAsync<QuestionSummaryDto>(
                specification,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return Result.Success(result);
        }
    }
}
