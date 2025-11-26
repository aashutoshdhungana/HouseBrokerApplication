using HouseBrokerApplication.Domain.Base;
using HouseBrokerApplication.Domain.DomainExceptions;

namespace HouseBrokerApplication.Domain.Aggregates.Listing
{
    public class Listing : Entity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Address Address { get; private set; }
        public decimal Price { get; private set; }
        public ListingType PropertyType { get; private set; }
        public string ContactPhone { get; private set; }
        public string ContactEmail { get; private set; }
        public ListingStatus Status { get; private set; }
        public int BrokerId { get; private set; }
        public UserInfo.UserInfo Broker { get; private set; }
        public IReadOnlyList<ListingImage> Images => _images.AsReadOnly();
        public IReadOnlyList<Deal> Deals => _deals.AsReadOnly();
        public IReadOnlyList<Offer> Offers => _offers.AsReadOnly();

        // Backing fields
        private List<ListingImage> _images = new();
        private List<Offer> _offers = new();
        private List<Deal> _deals = new();

        // For Ef Core
        private Listing() { }
        public Listing(string title, string description, Address address, decimal price,
                       ListingType propertyType, string contactPhone, string contactEmail, UserInfo.UserInfo broker)
        {
            Title = title;
            Description = description;
            Address = address;
            Price = price;
            PropertyType = propertyType;
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            Status = ListingStatus.Available;
            Broker = broker;
        }

        public void UpdateDetails(string title, string description, Address address, decimal price,
                                  ListingType propertyType, string contactPhone, string contactEmail)
        {
            Title = title;
            Description = description;
            Address = address;
            Price = price;
            PropertyType = propertyType;
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
        }

        public void AddImage(FileInfo.FileInfo fileInfo, bool isPrimary = false)
        {
            var prevPrimaryImage = _images.FirstOrDefault(i => i.IsPrimary);
            if (isPrimary && prevPrimaryImage is not null) prevPrimaryImage.SetPrimary(false);
            var listingImage = new ListingImage(this, fileInfo, isPrimary);
            _images.Add(listingImage);
        }

        public void RemoveImage(ListingImage listingImage)
        {
            if (listingImage.IsPrimary)
                _images.Where(i => i.Id != listingImage.Id)
                    .FirstOrDefault()?
                    .SetPrimary(true);

            _images.Remove(listingImage);
        }

        public void UpdatePrimaryImage(ListingImage listingImage)
        {
            var prevPrimaryImage = _images.FirstOrDefault(i => i.IsPrimary);
            if (prevPrimaryImage is not null) prevPrimaryImage.SetPrimary(false);
            listingImage.SetPrimary(true);
        }

        public void AddUpdateOffer(UserInfo.UserInfo userInfo, decimal offerPrice)
        {
            var existingOffer = _offers.FirstOrDefault(o => o.BuyerId == userInfo.Id);
            if (existingOffer is not null)
            {
                existingOffer.UpdateOfferAmount(offerPrice);
                return;
            }
            var newOffer = new Offer(this, userInfo, offerPrice);
            _offers.Add(newOffer);
        }

        public void RemoveOffer(int offerId)
        {
            if (_deals.FirstOrDefault(d => d.OfferId == offerId) is not null)
                throw new DomainException("Cannot remove an offer that has been accepted in a deal.");

            var offerToRemove = _offers.FirstOrDefault(o => o.Id == offerId);
            if (offerToRemove is null) return;
            _offers.Remove(offerToRemove);
        }

        public void AcceptOffer(Offer offer, decimal commission)
        {
            if (Status == ListingStatus.Sold || Status == ListingStatus.OffMarket)
                throw new DomainException("Cannot accept an offer on a unavailable listing");
            var deal = new Deal(this, offer, commission);
            _deals.Add(deal);
            Status = ListingStatus.Sold;
        }

        public void MarkAsOffMarket()
        {
            if (Status == ListingStatus.Sold)
                throw new DomainException("Cannot mark a sold listing as off-market.");
            Status = ListingStatus.OffMarket;
        }
    }
}
