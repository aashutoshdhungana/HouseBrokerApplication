using HouseBrokerApplication.Domain.Base;

namespace HouseBrokerApplication.Domain.Aggregates.GlobalConfig
{
    public class CommissionConfig : Entity, IAggregateRoot
    {
        public decimal StartingPrice { get; private set; }
        public decimal EndingPrice { get; private set; }
        public decimal CommissionRate { get; private set; }
    }
}
