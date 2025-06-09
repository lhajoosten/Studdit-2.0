using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Attributes;

namespace Studdit.Application.Questions.Commands.DeleteQuestion
{
    [RequireAuthentication]
    public class DeleteQuestionCommand : ICommand
    {
        public int Id { get; set; }
    }
}
