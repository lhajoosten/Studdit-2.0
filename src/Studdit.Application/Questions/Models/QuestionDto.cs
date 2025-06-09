using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Users.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Questions.Models
{
    public class QuestionDto : IMapFrom<Question>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public UserSummaryDto Author { get; set; } = new();
        public int VoteScore { get; set; }
        public int ViewCount { get; set; }
        public bool IsAnswered { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? ClosureReason { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        public int AnswerCount { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Question, QuestionDto>()
                .ForMember(d => d.AnswerCount, opt => opt.MapFrom(s => s.Answers.Count));
        }
    }
}
