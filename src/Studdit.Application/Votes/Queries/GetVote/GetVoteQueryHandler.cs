using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Votes.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Votes.Queries.GetVote
{
    public class GetVoteQueryHandler : IQueryHandler<GetVoteQuery, VoteDto>
    {
        private readonly IQueryRepository<Vote> _voteRepository;

        public GetVoteQueryHandler(IQueryRepository<Vote> voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<Result<VoteDto>> Handle(GetVoteQuery request, CancellationToken cancellationToken)
        {
            var vote = await _voteRepository.GetByIdProjectedAsync<VoteDto>(request.Id, cancellationToken);

            if (vote == null)
            {
                return Result.Failure<VoteDto>("Vote not found.");
            }

            return Result.Success(vote);
        }
    }
}