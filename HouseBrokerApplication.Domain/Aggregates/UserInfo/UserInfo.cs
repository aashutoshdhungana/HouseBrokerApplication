using HouseBrokerApplication.Domain.Base;

namespace HouseBrokerApplication.Domain.Aggregates.UserInfo
{
    public class UserInfo : Entity, IAggregateRoot
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string ContactPhone { get; private set; }
        public string ContactEmail { get; private set; }

        public UserInfo(string firstName, string lastName, string contactPhone, string contactEmail)
        {
            FirstName = firstName;
            LastName = lastName;
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
        }
    }
}
