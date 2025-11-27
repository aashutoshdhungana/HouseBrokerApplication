using HouseBrokerApplication.Domain.Base;

namespace HouseBrokerApplication.Domain.Aggregates.FileInfo
{
    public class FileInfo : AuditedEntity, IAggregateRoot
    {
        public string DisplayName { get; private set; }
        public string StoredName { get; private set; }
        public string Url { get; private set; }

        public FileInfo(string displayName, string storedName, string url)
        {
            DisplayName = displayName;
            StoredName = storedName;
            Url = url;
        }
    }
}
