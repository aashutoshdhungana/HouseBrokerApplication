namespace HouseBrokerApplication.Domain.Base
{
    public class Entity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public int CreatedBy { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public int? UpdatedBy { get; protected set; }
    }
}
