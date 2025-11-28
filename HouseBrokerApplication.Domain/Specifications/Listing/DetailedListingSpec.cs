using HouseBrokerApplication.Domain.Aggregates.Listing;
using HouseBrokerApplication.Domain.Base;
using System.Linq.Expressions;

namespace HouseBrokerApplication.Domain.Specifications.Listing
{
    public class DetailedListingSpec : BaseSpecification<Aggregates.Listing.Listing>
    {
        public DetailedListingSpec(Expression<Func<Aggregates.Listing.Listing, bool>> criteria) : base(criteria)
        {
            AddInclude(x => x.Broker);
            AddInclude($"{nameof(Aggregates.Listing.Listing.Images)}.{nameof(ListingImage.FileInfo)}");
            AddInclude($"{nameof(Aggregates.Listing.Listing.Offers)}.{nameof(Offer.Buyer)}");
            AddInclude($"{nameof(Aggregates.Listing.Listing.Offers)}.{nameof(Offer.Deal)}");
        }
        public DetailedListingSpec(Expression<Func<Aggregates.Listing.Listing, bool>> criteria, int skip, int take) : base(criteria)
        {
            AddInclude(x => x.Broker);
            AddInclude($"{nameof(Aggregates.Listing.Listing.Images)}.{nameof(ListingImage.FileInfo)}");
            AddInclude($"{nameof(Aggregates.Listing.Listing.Offers)}.{nameof(Offer.Buyer)}");
            AddInclude($"{nameof(Aggregates.Listing.Listing.Offers)}.{nameof(Offer.Deal)}");
            AddInclude($"{nameof(Aggregates.Listing.Listing.Deals)}.{nameof(Deal.Offer)}");
            Skip = skip;
            Take = take;
        }
    }
}
