using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;
using Studdit.Domain.ValueObjects;

namespace Studdit.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<User> _userQueryRepository;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IQueryRepository<User> userQueryRepository)
        {
            _unitOfWork = unitOfWork;
            _userQueryRepository = userQueryRepository;
        }

        public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if username already exists
            var usernameExists = await _userQueryRepository.AnyAsync(u => u.Username == request.Username, cancellationToken);
            if (usernameExists)
            {
                return Result.Failure<int>("Username already exists.");
            }

            // Check if email already exists
            var emailExists = await _userQueryRepository.AnyAsync(u => u.Email.Value == request.Email.ToLowerInvariant(), cancellationToken);
            if (emailExists)
            {
                return Result.Failure<int>("Email address already exists.");
            }

            try
            {
                var email = Email.Create(request.Email);

                // In a real application, you would hash the password here
                var passwordHash = HashPassword(request.Password);

                var user = User.Create(request.Username, email, passwordHash, request.DisplayName);

                if (!string.IsNullOrEmpty(request.Bio))
                {
                    user.UpdateProfile(request.DisplayName, request.Bio);
                }

                var repository = _unitOfWork.CommandRepository<User>();
                var createdUser = await repository.AddAsync(user, 0, cancellationToken); // System user for creation
                await _unitOfWork.SaveChangesAsync(0, cancellationToken);

                return Result.Success(createdUser.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>($"Failed to create user: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            // In a real application, use a proper password hashing library like BCrypt
            // This is just a placeholder
            return $"HASHED_{password}_{Guid.NewGuid()}";
        }
    }
}