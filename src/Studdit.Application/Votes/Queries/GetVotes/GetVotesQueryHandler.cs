using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Common.Specifications;
using Studdit.Application.Votes.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Votes.Queries.GetVotes
{
    public class GetVotesQueryHandler : IQueryHandler<GetVotesQuery, PaginatedList<VoteSummaryDto>>
    {
        private readonly IQueryRepository<Vote> _voteRepository;

        public GetVotesQueryHandler(IQueryRepository<Vote> voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<Result<PaginatedList<VoteSummaryDto>>> Handle(GetVotesQuery request, CancellationToken cancellationToken)
        {
            var specification = new BaseEntitySpecification<Vote>();

            // Apply filters
            if (request.UserId.HasValue)
            {
                specification = specification.Where(v => v.User.Id == request.UserId.Value);
            }

            if (request.QuestionId.HasValue)
            {
                specification = specification.Where(v => v.Question != null && v.Question.Id == request.QuestionId.Value);
            }

            if (request.AnswerId.HasValue)
            {
                specification = specification.Where(v => v.Answer != null && v.Answer.Id == request.AnswerId.Value);
            }

            if (!string.IsNullOrEmpty(request.VoteType))
            {
                specification = specification.Where(v => v.Type.Name == request.VoteType);
            }

            // Apply sorting
            specification = request.SortBy?.ToLowerInvariant() switch
            {
                "type" => request.SortDescending
                    ? specification.OrderByDescending(v => v.Type.Name)
                    : specification.OrderBy(v => v.Type.Name),
                _ => request.SortDescending
                    ? specification.OrderByDescending(v => v.CreatedDate)
                    : specification.OrderBy(v => v.CreatedDate)
            };

            // Apply pagination
            specification = specification.Paginate(request.PageNumber, request.PageSize);

            // Include related data
            specification = specification.Include(v => v.User).Include(v => v.Question).Include(v => v.Answer);

            var result = await _voteRepository.GetPaginatedProjectedAsync<VoteSummaryDto>(
                specification,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return Result.Success(result);
        }
    }
}