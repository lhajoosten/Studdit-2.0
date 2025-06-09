using Studdit.Application.Common.Abstractions;

namespace Studdit.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : ICommand
    {
        public int Id { get; set; }
    }
}
