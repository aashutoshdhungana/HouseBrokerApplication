using HouseBrokerApplication.Domain.Aggregates.Listing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApplication.Application.DTOs.Responses
{
    public class DetailedListing
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
        public List<OfferResponse> Offers { get; set; }
        public List<DealResponse> Deals { get; set; }
        public List<ImageResponse> Images { get; set; }

    }
}
