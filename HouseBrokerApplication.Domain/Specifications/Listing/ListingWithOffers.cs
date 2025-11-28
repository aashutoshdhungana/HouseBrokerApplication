using HouseBrokerApplication.Domain.Base;
using System.Linq.Expressions;

namespace HouseBrokerApplication.Domain.Specifications.Listing
{
    public class ListingWithOffers : BaseSpecification<Aggregates.Listing.Listing>
    {
        public ListingWithOffers(Expression<Func<Aggregates.Listing.Listing, bool>> criteria) : base(criteria)
        {
            AddInclude(x => x.Offers);
        }
    }
}
