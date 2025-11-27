namespace HouseBrokerApplication.Application.DTOs.Requests
{
    public class LoginReq
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsBrokerLogin { get; set; }
    }
}
