using Studdit.Application.Common.Abstractions;

namespace Studdit.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : ICommand
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Email { get; set; }
    }
}