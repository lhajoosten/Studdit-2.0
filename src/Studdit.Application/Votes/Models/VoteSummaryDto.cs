using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Domain.Entities;

namespace Studdit.Application.Votes.Models
{
    public class VoteSummaryDto : IMapFrom<Vote>
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public DateTime CreatedDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Vote, VoteSummaryDto>()
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.Name))
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.User.Id))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.User.Username))
                .ForMember(d => d.QuestionId, opt => opt.MapFrom(s => s.Question != null ? s.Question.Id : (int?)null))
                .ForMember(d => d.AnswerId, opt => opt.MapFrom(s => s.Answer != null ? s.Answer.Id : (int?)null));
        }
    }
}