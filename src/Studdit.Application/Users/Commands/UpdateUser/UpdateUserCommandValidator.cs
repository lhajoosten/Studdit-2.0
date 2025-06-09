using FluentValidation;

namespace Studdit.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User ID is required.");

            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .Length(1, 100).WithMessage("Display name must be between 1 and 100 characters.");

            RuleFor(x => x.Bio)
                .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Bio));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(320).WithMessage("Email cannot exceed 320 characters.")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}