using Studdit.Application.Common.Abstractions;
using Studdit.Domain.Entities;

namespace Studdit.Application.Questions.Models
{
    public class TagDto : IMapFrom<Tag>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UsageCount { get; set; }
    }
}
