using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Questions.Commands.UpdateQuestion
{
    [RequireAuthentication]
    public class UpdateQuestionCommand : ICommand
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> TagNames { get; set; } = new();
    }
}
