using FluentValidation;

namespace Studdit.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, underscores, and hyphens.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(320).WithMessage("Email cannot exceed 320 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit.");

            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .Length(1, 100).WithMessage("Display name must be between 1 and 100 characters.");

            RuleFor(x => x.Bio)
                .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Bio));
        }
    }
}