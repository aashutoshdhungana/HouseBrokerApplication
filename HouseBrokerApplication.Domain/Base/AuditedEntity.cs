namespace HouseBrokerApplication.Domain.Base
{
    public class AuditedEntity : Entity
    {
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public int CreatedBy { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public int? UpdatedBy { get; protected set; }

        public void SetCreated(int createdBy)
        {
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetUpdated(int updatedBy)
        {
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
