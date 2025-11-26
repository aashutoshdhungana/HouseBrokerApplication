using HouseBrokerApplication.Domain.Base;

namespace HouseBrokerApplication.Domain.Aggregates.Listing
{
    public class Deal : Entity
    {
        public int ListingId { get; private set; }
        public int OfferId { get; private set; }
        public DateTime DealDate { get; private set; }
        public decimal Commission { get; private set; }
        public Listing Listing { get; private set; }
        public Offer Offer { get; private set; }

        public Deal(Listing listing, Offer offer, decimal commission)
        {
            Listing = listing;
            Offer = offer;
            Commission = commission;
        }
    }
}
