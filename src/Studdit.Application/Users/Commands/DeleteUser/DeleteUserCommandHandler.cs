using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<User> _userQueryRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteUserCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<User> userQueryRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _userQueryRepository = userQueryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Get the user
            var user = await _userQueryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Check authorization - users can only delete their own account (unless admin)
            if (_currentUserService.UserId != user.Id)
            {
                return Result.Failure("You can only delete your own account.");
            }

            if (!user.IsActive)
            {
                return Result.Failure("User is already inactive.");
            }

            try
            {
                // Soft delete by deactivating the user
                user.Deactivate();

                var repository = _unitOfWork.CommandRepository<User>();
                await repository.UpdateAsync(user, _currentUserService.UserId ?? 0, cancellationToken);
                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId ?? 0, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete user: {ex.Message}");
            }
        }
    }
}
