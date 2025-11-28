using HouseBrokerApplication.Domain.Aggregates.UserInfo;

namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface ITokenService
    {
        Task<Result<(UserInfo, string)>> GenerateToken(string userName, string password, string clientId, string clientSecret, bool isBrokerLogin);
    }
}
