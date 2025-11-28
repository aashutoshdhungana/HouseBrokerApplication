using HouseBrokerApplication.Domain.Aggregates.UserInfo;

namespace HouseBrokerApplication.Application.DTOs.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
