using AutoMapper;
using Studdit.Application.Common.Abstractions;
using Studdit.Domain.Entities;

namespace Studdit.Application.Users.Models
{
    public class UserSummaryDto : IMapFrom<User>
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Reputation { get; set; }
        public bool IsActive { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserSummaryDto>();
        }
    }
}