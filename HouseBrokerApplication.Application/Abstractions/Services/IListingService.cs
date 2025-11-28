using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Application.DTOs.Responses;

namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface IListingService
    {
        Task<Result<ListingResponse>> CreateAsync(CreateUpdateListingRequest request);
        Task<Result<ListingResponse>> Update(int id, CreateUpdateListingRequest request);
        Task<Result<string>> DeleteAsync(int id);
        Task<Result<ListingResponse>> GetByIdAsync(int id);
        Task<Result<DetailedListing>> GetDetailedByIdAsync(int id);
        Task<Result<PaginationResponse<ListingResponse>>> GetListings(ListingFilters filters);
        Task<Result<PaginationResponse<ListingResponse>>> GetBrokerListing(int brokerId, ListingFilters filters);
        Task<Result<string>> UploadImage(int listingId, byte[] bytes, string imageName, bool isPrimary);
        Task<Result<string>> RemoveImage(int listingId, int imageId);
        Task<Result<string>> SetPrimaryImage(int listingId, int imageId);
        Task<Result<string>> AddUpdateOffer(int listingId, AddUpdateOffer offerPrice);
        Task<Result<string>> RemoveOffer(int listingId, int offerId);
        Task<Result<string>> AcceptOffer(int listingId, int offerId);
        Task<Result<string>> MarkAsOffMarket(int listingId);
    }
}
