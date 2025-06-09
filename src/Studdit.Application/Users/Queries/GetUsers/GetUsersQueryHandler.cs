using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Common.Specifications;
using Studdit.Application.Users.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PaginatedList<UserSummaryDto>>
    {
        private readonly IQueryRepository<User> _userRepository;

        public GetUsersQueryHandler(IQueryRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<PaginatedList<UserSummaryDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var specification = new BaseEntitySpecification<User>();

            // Apply filters
            if (request.IsActive.HasValue)
            {
                specification = specification.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                specification = specification.Where(u =>
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.DisplayName.ToLower().Contains(searchTerm));
            }

            // Apply sorting
            specification = request.SortBy?.ToLowerInvariant() switch
            {
                "username" => request.SortDescending
                    ? specification.OrderByDescending(u => u.Username)
                    : specification.OrderBy(u => u.Username),
                "displayname" => request.SortDescending
                    ? specification.OrderByDescending(u => u.DisplayName)
                    : specification.OrderBy(u => u.DisplayName),
                "createddate" => request.SortDescending
                    ? specification.OrderByDescending(u => u.CreatedDate)
                    : specification.OrderBy(u => u.CreatedDate),
                _ => request.SortDescending
                    ? specification.OrderByDescending(u => u.Reputation)
                    : specification.OrderBy(u => u.Reputation)
            };

            // Apply pagination
            specification = specification.Paginate(request.PageNumber, request.PageSize);

            var result = await _userRepository.GetPaginatedProjectedAsync<UserSummaryDto>(
                specification,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return Result.Success(result);
        }
    }
}
