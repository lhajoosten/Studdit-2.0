using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Application.Users.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IQueryRepository<User> _userRepository;

        public GetUserProfileQueryHandler(IQueryRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdProjectedAsync<UserProfileDto>(request.Id, cancellationToken);

            if (user == null)
            {
                return Result.Failure<UserProfileDto>("User not found.");
            }

            return Result.Success(user);
        }
    }
}
