using Studdit.Application.Common.Abstractions;
using Studdit.Application.Users.Models;

namespace Studdit.Application.Users.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IQuery<UserProfileDto>
    {
        public int Id { get; set; }
    }
}
