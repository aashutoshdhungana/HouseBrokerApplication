using HouseBrokerApplication.Application.Abstractions.Services;
using System.Security.Claims;

namespace HouseBrokerApplication.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal? _user;
        private readonly int? userId;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor?.HttpContext?.User;
            var userInfoIdClaim = _user?.Claims?.FirstOrDefault(x => x.Type == "userinfo_id");
            userId = userInfoIdClaim == null ? null : Convert.ToInt32(userInfoIdClaim.Value);
        }
        public int? UserId => userId;

        public ClaimsPrincipal? ClaimsPrincipal => _user;
    }
}
