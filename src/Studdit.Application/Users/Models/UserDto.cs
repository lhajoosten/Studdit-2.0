using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Domain.Entities;

namespace Studdit.Application.Users.Models
{
    public class UserDto : IMapFrom<User>
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int Reputation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value));
        }
    }
}