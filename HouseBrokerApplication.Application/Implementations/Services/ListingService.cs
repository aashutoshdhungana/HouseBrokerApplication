using FluentValidation;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Application.DTOs.Responses;
using HouseBrokerApplication.Domain.Aggregates.Listing;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using HouseBrokerApplication.Domain.Base;
using HouseBrokerApplication.Domain.DomainExceptions;
using HouseBrokerApplication.Domain.Specifications.Listing;
using Mapster;

namespace HouseBrokerApplication.Application.Implementations.Services
{
    public class ListingService(
        IRepository<Listing> listingRepository,
        IRepository<UserInfo> userRepository,
        IValidator<CreateUpdateListingRequest> createUpdateValidator,
        IFileService fileService,
        ICurrentUserService currentUserService,
        IComissionConfigService comissionConfigService)
        : IListingService
    {
        /// <summary>
        /// Accepts a specific offer, marking the listing as 'Sold' and creating a Deal.
        /// </summary>
        public async Task<Result<string>> AcceptOffer(int listingId, int offerId)
        {
            if (!currentUserService.UserId.HasValue)
                return Result<string>.Unauthorized("User is not logged in");

            // Fetch listing with offers to ensure the offer is available for acceptance
            var listingSpec = new ListingWithOffers(x => x.Id == listingId);
            var listing = await listingRepository.GetSingleBySpecification(listingSpec);

            if (listing is null)
                return Result<string>.NotFound();

            // Check if the current user is the broker who owns the listing
            if (listing.BrokerId != currentUserService.UserId.Value)
                return Result<string>.Unauthorized("Only the broker of the listing can accept offers.");

            var offerToAccept = listing.Offers.FirstOrDefault(o => o.Id == offerId);

            if (offerToAccept is null)
                return Result<string>.NotFound();

            var calculatedComission = await comissionConfigService.CalculateCommission(offerToAccept.OfferAmount);
            try
            {
                // Domain Logic: Accept the offer, update status, create deal
                listing.AcceptOffer(offerToAccept, calculatedComission);
                listingRepository.Update(listing);

                var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();

                if (!isSuccess)
                    return Result<string>.Failure("Failed to accept offer and close deal.");

                return Result<string>.Success("Offer accepted and listing marked as Sold.", string.Empty);
            }
            catch (DomainException ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new offer or updates an existing offer by the same user.
        /// </summary>
        public async Task<Result<string>> AddUpdateOffer(int listingId, AddUpdateOffer request)
        {
            if (!currentUserService.UserId.HasValue)
                return Result<string>.Unauthorized("User is not logged in");

            var listing = await listingRepository.GetByIdAsync(listingId);
            if (listing is null)
                return Result<string>.NotFound();

            var buyerInfo = await userRepository.GetByIdAsync(currentUserService.UserId.Value);
            if (buyerInfo is null)
                return Result<string>.NotFound();

            if (listing.BrokerId == buyerInfo.Id)
                return Result<string>.Failure("Cannot add offer in own listing");

            try
            {
                // Domain Logic: Add or update the offer
                listing.AddUpdateOffer(buyerInfo, request.OfferPrice);
                listingRepository.Update(listing);

                var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();

                if (!isSuccess)
                    return Result<string>.Failure("Failed to add or update offer.");

                return Result<string>.Success("Offer added/updated successfully.", string.Empty);
            }
            catch (DomainException ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }


        public async Task<Result<ListingResponse>> CreateAsync(CreateUpdateListingRequest request)
        {
            var validationResult = createUpdateValidator.Validate(request);
            if (!validationResult.IsValid) return validationResult.Errors.ValidationError<ListingResponse>();

            if (!currentUserService.UserId.HasValue)
                return Result<ListingResponse>.Unauthorized("User is not logged in");
            var listing = new Listing(
                request.Title,
                request.Description,
                request.Address,
                request.Price,
                request.PropertyType,
                request.ContactPhone,
                request.ContactEmail,
                currentUserService.UserId.Value);

            listingRepository.Add(listing);
            var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();
            if (!isSuccess) return Result<ListingResponse>.Failure("Failed to add listing");
            var responseObject = listing.Adapt<ListingResponse>();
            return Result<ListingResponse>.Success("Added successfully", responseObject);
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            if (!currentUserService.UserId.HasValue)
                return Result<string>.Unauthorized("User is not logged in");

            var lisitng = await listingRepository.GetByIdAsync(id);
            if (lisitng is null)
                return Result<string>.NotFound();

            listingRepository.Delete(lisitng);
            var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();
            if (!isSuccess) return Result<string>.Failure("Failed to delete listing");

            return Result<string>.Success("Successful", string.Empty);
        }

        /// <summary>
        /// Gets all listings created by a specific broker, applying pagination and filters.
        /// </summary>
        public async Task<Result<PaginationResponse<ListingResponse>>> GetBrokerListing(int brokerId, ListingFilters filters)
        {
            var listingSpecs = new ListingWithBrokerAndImages(x =>
                x.BrokerId == brokerId &&
                (!filters.HighPrice.HasValue || x.Price <= filters.HighPrice) &&
                (!filters.LowPrice.HasValue || x.Price >= filters.LowPrice) &&
                (!filters.ListingType.HasValue || filters.ListingType == x.PropertyType)
            , filters.Skip ?? 0, filters.Take ?? 10);

            var listings = await listingRepository.GetBySpecification(listingSpecs);
            var mappedResponse = listings.Adapt<List<ListingResponse>>();
            var paginationResponse = new PaginationResponse<ListingResponse>()
            {
                Data = mappedResponse,
                PageNo = (filters.Skip ?? 0) + 1,
                PageCount = mappedResponse.Count
            };

            return Result<PaginationResponse<ListingResponse>>.Success(
                "Success",
                paginationResponse
            );
        }

        public async Task<Result<ListingResponse>> GetByIdAsync(int id)
        {
            var listingSpec = new ListingWithBrokerAndImages(x => x.Id == id);
            var listingResponse = await listingRepository.GetSingleBySpecification(listingSpec);
            if (listingResponse is null)
                return Result<ListingResponse>.NotFound();

            var adaptedResponse = listingResponse.Adapt<ListingResponse>();
            return Result<ListingResponse>.Success("Successful", adaptedResponse);
        }

        public async Task<Result<DetailedListing>> GetDetailedByIdAsync(int id)
        {
            var listingSpec = new DetailedListingSpec(x => x.Id == id);
            var listing = await listingRepository.GetSingleBySpecification(listingSpec);

            if (listing is null)
                return Result<DetailedListing>.NotFound();

            var adaptedResponse = listing.Adapt<DetailedListing>();
            if (currentUserService.UserId.HasValue && listing.BrokerId == currentUserService.UserId.Value)
            {
                // Broker sees all details, including offers/deals
                foreach(var offer in adaptedResponse.Offers)
                {
                    offer.EstimatedCommission = await comissionConfigService.CalculateCommission(offer.OfferAmount);
                }
                return Result<DetailedListing>.Success("Successful", adaptedResponse);
            }
            else
            {
                // Non-broker/public view: remove sensitive/private information
                adaptedResponse.Offers = new List<OfferResponse>();
                adaptedResponse.Deals = new List<DealResponse>();
                return Result<DetailedListing>.Success("Successful", adaptedResponse);
            }
        }

        public async Task<Result<PaginationResponse<ListingResponse>>> GetListings(ListingFilters filters)
        {
            var listingSpecs = new ListingWithBrokerAndImages(x =>
                (!filters.HighPrice.HasValue || x.Price <= filters.HighPrice) &&
                (!filters.LowPrice.HasValue || x.Price >= filters.LowPrice) &&
                (!filters.ListingType.HasValue || filters.ListingType == x.PropertyType)
            , filters.Skip ?? 0, filters.Take ?? 10);

            var listings = await listingRepository.GetBySpecification(listingSpecs);
            var mappedResponse = listings.Adapt<List<ListingResponse>>();
            var paginationResponse = new PaginationResponse<ListingResponse>()
            {
                Data = mappedResponse,
                PageNo = (filters.Skip ?? 0) + 1,
                PageCount = mappedResponse.Count
            };

            return Result<PaginationResponse<ListingResponse>>.Success(
                "Success",
                paginationResponse
            );
        }

        public async Task<Result<string>> MarkAsOffMarket(int listingId)
        {
            if (!currentUserService.UserId.HasValue)
                return Result<string>.Unauthorized("User is not logged in");

            var listing = await listingRepository.GetByIdAsync(listingId);

            if (listing is null)
                return Result<string>.NotFound();

            // Check authorization
            if (listing.BrokerId != currentUserService.UserId.Value)
                return Result<string>.Unauthorized("Only the broker of the listing can mark it as off-market.");

            try
            {
                listing.MarkAsOffMarket();
                listingRepository.Update(listing);

                var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();

                if (!isSuccess)
                    return Result<string>.Failure("Failed to mark listing as off-market.");

                return Result<string>.Success("Listing marked as off-market successfully.", string.Empty);
            }
            catch (DomainException ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }

        public async Task<Result<string>> RemoveImage(int listingId, int imageId)
        {
            if (!currentUserService.UserId.HasValue)
                return Result<string>.Unauthorized("User is not logged in");

            // Fetch listing with images
            var listingSpec = new ListingWithBrokerAndImages(x => x.Id == listingId);
            var listing = await listingRepository.GetSingleBySpecification(listingSpec);

            if (listing is null)
                return Result<string>.NotFound();

            // Check authorization
            if (listing.BrokerId != currentUserService.UserId.Value)
                return Result<string>.Unauthorized("Only the broker of the listing can remove images.");

            var imageToRemove = listing.Images.FirstOrDefault(i => i.Id == imageId);

            if (imageToRemove is null)
                return Result<string>.NotFound();

            try
            {
                listing.RemoveImage(imageToRemove);
                listingRepository.Update(listing);

                var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();

                if (!isSuccess)
                    return Result<string>.Failure("Failed to remove image from database.");

                return Result<string>.Success("Image removed successfully.", string.Empty);
            }
            catch (DomainException ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }

        public async Task<Result<string>> RemoveOffer(int listingId, int offerId)
        {
            if (!currentUserService.UserId.HasValue)
                return Result<string>.Unauthorized("User is not logged in");

            // Fetch listing with offers
            var listingSpec = new ListingWithOffers(x => x.Id == listingId);
            var listing = await listingRepository.GetSingleBySpecification(listingSpec);

            if (listing is null)
                return Result<string>.NotFound();

            // Check authorization: Only the broker OR the user who made the offer can remove it.
            var offerToRemove = listing.Offers.FirstOrDefault(o => o.Id == offerId);

            if (offerToRemove is null)
                return Result<string>.NotFound();

            // Broker ID is listing.BrokerId, Buyer ID is offerToRemove.BuyerId
            var currentUserId = currentUserService.UserId.Value;

            if (currentUserId != listing.BrokerId && currentUserId != offerToRemove.BuyerId)
                return Result<string>.Unauthorized("Only the broker or the offer creator can remove the offer.");

            try
            {
                listing.RemoveOffer(offerId);
                listingRepository.Update(listing);

                var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();

                if (!isSuccess)
                    return Result<string>.Failure("Failed to remove offer.");

                return Result<string>.Success("Offer removed successfully.", string.Empty);
            }
            catch (DomainException ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }

        public async Task<Result<string>> SetPrimaryImage(int listingId, int imageId)
        {
            if (!currentUserService.UserId.HasValue)
                return Result<string>.Unauthorized("User is not logged in");

            // Fetch listing with images
            var listingSpec = new ListingWithBrokerAndImages(x => x.Id == listingId);
            var listing = await listingRepository.GetSingleBySpecification(listingSpec);

            if (listing is null)
                return Result<string>.NotFound();

            // Check authorization
            if (listing.BrokerId != currentUserService.UserId.Value)
                return Result<string>.Unauthorized("Only the broker of the listing can set the primary image.");

            var imageToSetPrimary = listing.Images.FirstOrDefault(i => i.Id == imageId);

            if (imageToSetPrimary is null)
                return Result<string>.NotFound();

            try
            {
                listing.UpdatePrimaryImage(imageToSetPrimary);
                listingRepository.Update(listing);

                var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();

                if (!isSuccess)
                    return Result<string>.Failure("Failed to set primary image.");

                return Result<string>.Success("Primary image set successfully.", string.Empty);
            }
            catch (DomainException ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }

        public async Task<Result<ListingResponse>> Update(int id, CreateUpdateListingRequest request)
        {
            var validationResult = createUpdateValidator.Validate(request);
            if (!validationResult.IsValid) return validationResult.Errors.ValidationError<ListingResponse>();

            if (!currentUserService.UserId.HasValue)
                return Result<ListingResponse>.Unauthorized("User is not logged in");

            var listing = await listingRepository.GetByIdAsync(id);

            if (listing is null)
                return Result<ListingResponse>.NotFound();

            // Check authorization
            if (listing.BrokerId != currentUserService.UserId.Value)
                return Result<ListingResponse>.Unauthorized("Only the broker of the listing can update it.");

            try
            {
                // Domain Logic: Update listing details
                listing.UpdateDetails(
                    request.Title,
                    request.Description,
                    request.Address,
                    request.Price,
                    request.PropertyType,
                    request.ContactPhone,
                    request.ContactEmail);

                listingRepository.Update(listing);

                var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();

                if (!isSuccess) return Result<ListingResponse>.Failure("Failed to update listing");

                var responseObject = listing.Adapt<ListingResponse>();
                return Result<ListingResponse>.Success("Updated successfully", responseObject);
            }
            catch (DomainException ex)
            {
                return Result<ListingResponse>.Failure(ex.Message);
            }
        }

        public async Task<Result<string>> UploadImage(int listingId, byte[] bytes, string imageName, bool isPrimary)
        {
            string extension = Path.GetExtension(imageName).ToLowerInvariant();
            bool isImage = extension == ".jpg" ||
                   extension == ".jpeg" ||
                   extension == ".png" ||
                   extension == ".gif" ||
                   extension == ".bmp" ||
                   extension == ".tiff" ||
                   extension == ".webp";

            if (!isImage)
                return Result<string>.Failure("Invalid file type uploded");

            var uploadedFileResult = await fileService.UploadAsync(bytes, imageName);
            if (!uploadedFileResult.IsSuccess)
                return Result<string>.Failure("Failed to upload image");

            var listingWithImages = new ListingWithBrokerAndImages(x => x.Id == listingId);
            var listing = await listingRepository.GetSingleBySpecification(listingWithImages)
                ;
            if (listing == null) return Result<string>.Failure("Failed to add image");
            listing.AddImage(uploadedFileResult.Data!, isPrimary);
            listingRepository.Update(listing);
            
            var isSuccess = await listingRepository.UnitOfWork.SaveChangesAsync();
            if (!isSuccess) return Result<string>.Failure("Failed to add image");

            return Result<string>.Success("Successfull", uploadedFileResult?.Data?.Url ?? string.Empty);
        }
    }
}
