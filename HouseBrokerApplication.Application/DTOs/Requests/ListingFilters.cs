using HouseBrokerApplication.Domain.Aggregates.Listing;

namespace HouseBrokerApplication.Application.DTOs.Requests
{
    public class ListingFilters
    {
        public decimal? LowPrice { get; set; }
        public decimal? HighPrice { get; set; }
        public ListingType? ListingType { get; set; }
        public int? Skip { get; set; } = 0;
        public int? Take { get; set; } = 10;
    }
}
