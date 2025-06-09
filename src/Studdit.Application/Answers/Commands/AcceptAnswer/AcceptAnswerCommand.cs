using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Answers.Commands.AcceptAnswer
{
    [RequireAuthentication]
    public class AcceptAnswerCommand : ICommand
    {
        public int AnswerId { get; set; }
    }
}