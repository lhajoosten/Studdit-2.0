using Studdit.Application.Common.Abstractions;
using Studdit.Application.Users.Models;

namespace Studdit.Application.Users.Queries.GetUser
{
    public class GetUserQuery : IQuery<UserDto>
    {
        public int Id { get; set; }
    }
}
