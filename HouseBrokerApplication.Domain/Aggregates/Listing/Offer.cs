using HouseBrokerApplication.Domain.Base;

namespace HouseBrokerApplication.Domain.Aggregates.Listing
{
    public class Offer : AuditedEntity
    {
        public int BuyerId { get; private set; }
        public int ListingId { get; private set; }
        public decimal OfferAmount { get; private set; }
        public UserInfo.UserInfo Buyer { get; private set; }
        public Listing Listing { get; private set; }
        public Deal? Deal { get; private set; }
        private Offer() { }
        public Offer(Listing listing, UserInfo.UserInfo buyer, decimal offerAmount)
        {
            Listing = listing;
            Buyer = buyer;
            OfferAmount = offerAmount;
        }

        public void UpdateOfferAmount(decimal newAmount)
        {
            OfferAmount = newAmount;
        }
    }
}
