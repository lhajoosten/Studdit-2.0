using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Answers.Commands.CreateAnswer
{
    [RequireAuthentication]
    public class CreateAnswerCommand : ICommand<int>
    {
        public string Content { get; set; } = string.Empty;
        public int QuestionId { get; set; }
    }
}