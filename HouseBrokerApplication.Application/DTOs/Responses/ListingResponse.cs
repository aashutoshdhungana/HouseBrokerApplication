using HouseBrokerApplication.Domain.Aggregates.Listing;

namespace HouseBrokerApplication.Application.DTOs.Responses
{
    public class ListingResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
        public decimal Price { get; set; }
        public ListingType PropertyType { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public int BrokerId { get; set; }
        public string BrokerFullName { get; set; }
        public ListingStatus Status { get; set; }
        public string PrimaryImageUrl { get; set; }
    }
}
