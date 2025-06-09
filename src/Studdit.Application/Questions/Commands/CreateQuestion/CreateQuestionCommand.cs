using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Questions.Commands.CreateQuestion
{
    [RequireAuthentication]
    public class CreateQuestionCommand : ICommand<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> TagNames { get; set; } = new();
    }
}
