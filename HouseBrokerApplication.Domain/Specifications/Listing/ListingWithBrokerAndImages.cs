using HouseBrokerApplication.Domain.Aggregates.Listing;
using HouseBrokerApplication.Domain.Base;
using System.Linq.Expressions;

namespace HouseBrokerApplication.Domain.Specifications.Listing
{
    public class ListingWithBrokerAndImages : BaseSpecification<Aggregates.Listing.Listing>
    {
        public ListingWithBrokerAndImages(Expression<Func<Aggregates.Listing.Listing, bool>> criteria) : base(criteria)
        {
            AddInclude(x => x.Broker);
            AddInclude($"{nameof(Aggregates.Listing.Listing.Images)}.{nameof(ListingImage.FileInfo)}");
        }
        public ListingWithBrokerAndImages(Expression<Func<Aggregates.Listing.Listing, bool>> criteria, int skip, int take) : base(criteria)
        {
            AddInclude(x => x.Broker);
            AddInclude($"{nameof(Aggregates.Listing.Listing.Images)}.{nameof(ListingImage.FileInfo)}");
            Skip = skip;
            Take = take;
        }
    }
}
