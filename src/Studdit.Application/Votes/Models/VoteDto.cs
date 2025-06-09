using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Users.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Votes.Models
{
    public class VoteDto : IMapFrom<Vote>
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public UserSummaryDto User { get; set; } = new();
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Vote, VoteDto>()
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.Name))
                .ForMember(d => d.QuestionId, opt => opt.MapFrom(s => s.Question != null ? s.Question.Id : (int?)null))
                .ForMember(d => d.AnswerId, opt => opt.MapFrom(s => s.Answer != null ? s.Answer.Id : (int?)null));
        }
    }
}