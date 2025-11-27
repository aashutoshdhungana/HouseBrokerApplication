using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using Microsoft.AspNetCore.Identity;

namespace HouseBrokerApplication.Infrastructure.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public AppUser(string username, UserInfo userInfo) : base(username)
        {
            UserInfo = userInfo;
        }
        public bool IsBroker { get; set; }
        public bool IsHomeSeeker { get; set; }
        public int UserInfoId { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
