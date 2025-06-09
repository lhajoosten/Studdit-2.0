using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Domain.Entities;

namespace Studdit.Application.Users.Models
{
    public class UserProfileDto : IMapFrom<User>
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int Reputation { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int QuestionCount { get; set; }
        public int AnswerCount { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserProfileDto>()
                .ForMember(d => d.QuestionCount, opt => opt.MapFrom(s => s.Questions.Count))
                .ForMember(d => d.AnswerCount, opt => opt.MapFrom(s => s.Answers.Count));
        }
    }
}