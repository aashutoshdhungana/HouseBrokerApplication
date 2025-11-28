using HouseBrokerApplication.API.Extensions;
using HouseBrokerApplication.API.Models;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.Constants;
using HouseBrokerApplication.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseBrokerApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController(
        IListingService listingService
        ) : ControllerBase
    {
        // Added XML comments to provide Swagger documentation for each endpoint

        /// <summary>
        /// Create a new listing.
        /// </summary>
        /// <param name="request">Listing creation payload.</param>
        /// <returns>Result containing the created listing.</returns>
        [HttpPost]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> Create(CreateUpdateListingRequest request)
        {
            var result = await listingService.CreateAsync(request);
            return result.HttpResponse();
        }

        /// <summary>
        /// Update an existing listing.
        /// </summary>
        /// <param name="id">Listing identifier.</param>
        /// <param name="request">Updated listing payload.</param>
        /// <returns>Result containing the updated listing.</returns>
        [HttpPut("/{id}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> Update(int id, CreateUpdateListingRequest request)
        {
            var result = await listingService.Update(id, request);
            return result.HttpResponse();
        }

        /// <summary>
        /// Delete a listing.
        /// </summary>
        /// <param name="id">Listing identifier.</param>
        /// <returns>Result indicating success or failure.</returns>
        [HttpDelete("{id}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await listingService.DeleteAsync(id);
            return result.HttpResponse();
        }

        /// <summary>
        /// Get a listing by id.
        /// </summary>
        /// <param name="id">Listing identifier.</param>
        /// <returns>Result containing the listing.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await listingService.GetByIdAsync(id);
            return result.HttpResponse();
        }

        /// <summary>
        /// Get detailed listing information (broker only).
        /// </summary>
        /// <param name="id">Listing identifier.</param>
        /// <returns>Result containing detailed listing data.</returns>
        [HttpGet("{id}/detailed")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> GetDetailedById(int id)
        {
            var result = await listingService.GetDetailedByIdAsync(id);
            return result.HttpResponse();
        }

        /// <summary>
        /// Get listings with optional filters and pagination.
        /// </summary>
        /// <param name="filters">Query filters for listings.</param>
        /// <returns>Result containing a paginated list of listings.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListings([FromQuery] ListingFilters filters)
        {
            var result = await listingService.GetListings(filters);
            return result.HttpResponse();
        }

        /// <summary>
        /// Upload an image for a listing. Supports multipart/form-data.
        /// </summary>
        /// <param name="listingId">Listing identifier.</param>
        /// <param name="request">Upload request containing file and primary flag.</param>
        /// <returns>Result indicating upload status and image URL when successful.</returns>
        [HttpPost("{listingId}/image")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int listingId, [FromForm] UploadImageRequest request)
        {
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            var result = await listingService.UploadImage(
                listingId,
                bytes,
                request.File.FileName,
                request.IsPrimary
            );
            return result.HttpResponse();
        }

        /// <summary>
        /// Add or update an offer for a listing.
        /// </summary>
        /// <param name="listingId">Listing identifier.</param>
        /// <param name="request">Offer payload containing the offer price.</param>
        /// <returns>Result indicating the operation status.</returns>
        [HttpPost("{listingId}/offer")]
        [Authorize]
        public async Task<IActionResult> AddUpdateOffer(int listingId, [FromBody] AddUpdateOffer request)
        {
            var result = await listingService.AddUpdateOffer(listingId, request);
            return result.HttpResponse();
        }

        /// <summary>
        /// Remove an offer from a listing.
        /// </summary>
        /// <param name="listingId">Listing identifier.</param>
        /// <param name="offerId">Offer identifier.</param>
        /// <returns>Result indicating the operation status.</returns>
        [HttpDelete("{listingId}/offer/{offerId}")]
        [Authorize]
        public async Task<IActionResult> RemoveOffer(int listingId, int offerId)
        {
            var result = await listingService.RemoveOffer(listingId, offerId);
            return result.HttpResponse();
        }

        /// <summary>
        /// Accept an offer and create a deal (broker only).
        /// </summary>
        /// <param name="listingId">Listing identifier.</param>
        /// <param name="offerId">Offer identifier to accept.</param>
        /// <returns>Result indicating the deal creation status.</returns>
        [HttpPost("{listingId}/deal/accept/{offerId}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> AcceptOffer(int listingId, int offerId)
        {
            var result = await listingService.AcceptOffer(listingId, offerId);
            return result.HttpResponse();
        }

        /// <summary>
        /// Mark a listing as off-market (broker only).
        /// </summary>
        /// <param name="listingId">Listing identifier.</param>
        /// <returns>Result indicating the operation status.</returns>
        [HttpPut("{listingId}/status/offmarket")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> MarkAsOffMarket(int listingId)
        {
            var result = await listingService.MarkAsOffMarket(listingId);
            return result.HttpResponse();
        }

        /// <summary>
        /// Remove an image from a listing (broker only).
        /// </summary>
        /// <param name="listingId">Listing identifier.</param>
        /// <param name="imageId">Image identifier.</param>
        /// <returns>Result indicating the operation status.</returns>
        [HttpDelete("{listingId}/image/{imageId}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> RemoveImage(int listingId, int imageId)
        {
            var result = await listingService.RemoveImage(listingId, imageId);
            return result.HttpResponse();
        }

        /// <summary>
        /// Set an image as the primary image for a listing (broker only).
        /// </summary>
        /// <param name="listingId">Listing identifier.</param>
        /// <param name="imageId">Image identifier to set as primary.</param>
        /// <returns>Result indicating the operation status.</returns>
        [HttpPut("{listingId}/image/{imageId}/primary")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> SetPrimaryImage(int listingId, int imageId)
        {
            var result = await listingService.SetPrimaryImage(listingId, imageId);
            return result.HttpResponse();
        }
    }
}
