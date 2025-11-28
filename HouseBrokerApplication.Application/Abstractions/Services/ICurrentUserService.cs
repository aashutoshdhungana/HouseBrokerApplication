using System.Security.Claims;

namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        ClaimsPrincipal? ClaimsPrincipal { get; }
    }
}
