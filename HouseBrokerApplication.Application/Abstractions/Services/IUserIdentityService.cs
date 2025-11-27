using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;

namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface IUserIdentityService
    {
        Task<Result<UserInfo>> RegisterUserAsync(RegisterUserReq request);
        Task<Result<UserInfo>> Login(LoginReq request);
    }
}
