using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Users.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Answers.Models
{
    public class AnswerDto : IMapFrom<Answer>
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public UserSummaryDto Author { get; set; } = new();
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public int VoteScore { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Answer, AnswerDto>()
                .ForMember(d => d.QuestionId, opt => opt.MapFrom(s => s.Question.Id))
                .ForMember(d => d.QuestionTitle, opt => opt.MapFrom(s => s.Question.Title));
        }
    }
}