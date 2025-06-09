using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Answers.Commands.UpdateAnswer
{
    [RequireAuthentication]
    public class UpdateAnswerCommand : ICommand
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}