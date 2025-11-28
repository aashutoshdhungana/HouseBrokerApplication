namespace HouseBrokerApplication.Application.DTOs.Requests
{
    public class LoginReq
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public bool IsBrokerLogin { get; set; }
    }
}
