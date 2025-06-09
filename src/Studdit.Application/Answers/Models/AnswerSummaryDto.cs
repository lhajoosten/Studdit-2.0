using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Users.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Answers.Models
{
    public class AnswerSummaryDto : IMapFrom<Answer>
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public UserSummaryDto Author { get; set; } = new();
        public int QuestionId { get; set; }
        public int VoteScore { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime CreatedDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Answer, AnswerSummaryDto>()
                .ForMember(d => d.QuestionId, opt => opt.MapFrom(s => s.Question.Id))
                .ForMember(d => d.Content, opt => opt.MapFrom(s => s.Content.Length > 150 ? s.Content.Substring(0, 150) + "..." : s.Content));
        }
    }
}