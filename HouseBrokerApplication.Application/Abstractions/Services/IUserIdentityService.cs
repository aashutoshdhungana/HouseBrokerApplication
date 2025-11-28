using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Application.DTOs.Responses;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;

namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface IUserIdentityService
    {
        Task<Result<UserInfo>> RegisterUserAsync(RegisterUserReq request);
        Task<Result<LoginResponse>> LoginAsync(LoginReq request);
        Task<Result<string>> RegisterProfileAsync(bool isBrokerPrfile);
    }
}
