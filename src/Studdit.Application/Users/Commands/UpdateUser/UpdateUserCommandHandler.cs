using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;
using Studdit.Domain.ValueObjects;

namespace Studdit.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<User> _userQueryRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<User> userQueryRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _userQueryRepository = userQueryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Get the user
            var user = await _userQueryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Check authorization - users can only update their own profile (unless admin)
            if (_currentUserService.UserId != user.Id)
            {
                return Result.Failure("You can only update your own profile.");
            }

            if (!user.IsActive)
            {
                return Result.Failure("Cannot update inactive user.");
            }

            try
            {
                // Update profile information
                user.UpdateProfile(request.DisplayName, request.Bio);

                // Update email if provided
                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email.Value)
                {
                    // Check if new email already exists
                    var emailExists = await _userQueryRepository.AnyAsync(
                        u => u.Email.Value == request.Email.ToLowerInvariant() && u.Id != request.Id,
                        cancellationToken);

                    if (emailExists)
                    {
                        return Result.Failure("Email address already exists.");
                    }

                    var newEmail = Email.Create(request.Email);
                    user.UpdateEmail(newEmail);
                }

                var repository = _unitOfWork.CommandRepository<User>();
                await repository.UpdateAsync(user, _currentUserService.UserId ?? 0, cancellationToken);
                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId ?? 0, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update user: {ex.Message}");
            }
        }
    }
}