namespace HouseBrokerApplication.Application.DTOs.Requests
{
    public class RegisterUserReq
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool RegisterAsBroker { get; set; }
    }
}
