using Studdit.Application.Common.Abstractions;

namespace Studdit.Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : ICommand<int>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
    }
}