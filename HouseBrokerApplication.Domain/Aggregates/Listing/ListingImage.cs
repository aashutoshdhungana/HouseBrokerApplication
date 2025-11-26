using HouseBrokerApplication.Domain.Base;

namespace HouseBrokerApplication.Domain.Aggregates.Listing
{
    public class ListingImage : Entity
    {
        public int ListingId { get; private set; }
        public int FileInfoId { get; private set; }
        public bool IsPrimary { get; private set; }
        public Listing Listing { get; private set; }
        public FileInfo.FileInfo FileInfo { get; private set; }

        private ListingImage() { }

        public ListingImage(Listing listing, FileInfo.FileInfo fileInfo, bool isPrimary = false)
        {
            Listing = listing;
            ListingId = listing.Id;
            FileInfo = fileInfo;
            FileInfoId = fileInfo.Id;
            IsPrimary = isPrimary;
        }

        public void SetPrimary(bool isPrimary)
        {
            IsPrimary = isPrimary;
        }
    }
}
