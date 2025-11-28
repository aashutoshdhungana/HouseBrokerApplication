using HouseBrokerApplication.Domain.Aggregates.Listing;

namespace HouseBrokerApplication.Application.DTOs.Requests
{
    public class CreateUpdateListingRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
        public decimal Price { get; set; }
        public ListingType PropertyType { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
    }
}
