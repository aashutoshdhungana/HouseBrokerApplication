using HouseBrokerApplication.Domain.Aggregates.UserInfo;

namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface ITokenService
    {
        string GenerateToken(UserInfo user, string clientId, string scopes, bool isBrokerLogin);
    }
}
