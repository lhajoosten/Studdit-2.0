using Studdit.Application.Common.Interfaces;
using System.Security.Claims;

namespace Studdit.Api.Services
{
    public class HttpContextCurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub") ??
                                 _httpContextAccessor.HttpContext?.User?.FindFirst("userId") ??
                                 _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

                return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : null;
            }
        }

        public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value ??
                                  _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
       
        public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }
}
