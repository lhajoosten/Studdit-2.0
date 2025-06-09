using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Users.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
    {
        private readonly IQueryRepository<User> _userRepository;

        public GetUserQueryHandler(IQueryRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdProjectedAsync<UserDto>(request.Id, cancellationToken);

            if (user == null)
            {
                return Result.Failure<UserDto>("User not found.");
            }

            return Result.Success(user);
        }
    }
}
