using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Answers.Commands.DeleteAnswer
{
    [RequireAuthentication]
    public class DeleteAnswerCommand : ICommand
    {
        public int Id { get; set; }
    }
}