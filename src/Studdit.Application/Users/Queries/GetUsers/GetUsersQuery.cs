using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Models;
using Studdit.Application.Users.Models;

namespace Studdit.Application.Users.Queries.GetUsers
{
    public class GetUsersQuery : IQuery<PaginatedList<UserSummaryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "Reputation";
        public bool SortDescending { get; set; } = true;
    }
}
