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
        [HttpPost]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> Create(CreateUpdateListingRequest request)
        {
            var result = await listingService.CreateAsync(request);
            return result.HttpResponse();
        }

        [HttpPut("/{id}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> Update(int id, CreateUpdateListingRequest request)
        {
            var result = await listingService.Update(id, request);
            return result.HttpResponse();
        }

        [HttpDelete("{id}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await listingService.DeleteAsync(id);
            return result.HttpResponse();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await listingService.GetByIdAsync(id);
            return result.HttpResponse();
        }

        [HttpGet("{id}/detailed")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> GetDetailedById(int id)
        {
            var result = await listingService.GetDetailedByIdAsync(id);
            return result.HttpResponse();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetListings([FromQuery] ListingFilters filters)
        {
            var result = await listingService.GetListings(filters);
            return result.HttpResponse();
        }

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

        [HttpPost("{listingId}/offer")]
        [Authorize]
        public async Task<IActionResult> AddUpdateOffer(int listingId, [FromBody] AddUpdateOffer request)
        {
            var result = await listingService.AddUpdateOffer(listingId, request);
            return result.HttpResponse();
        }

        [HttpDelete("{listingId}/offer/{offerId}")]
        [Authorize]
        public async Task<IActionResult> RemoveOffer(int listingId, int offerId)
        {
            var result = await listingService.RemoveOffer(listingId, offerId);
            return result.HttpResponse();
        }

        [HttpPost("{listingId}/deal/accept/{offerId}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> AcceptOffer(int listingId, int offerId)
        {
            var result = await listingService.AcceptOffer(listingId, offerId);
            return result.HttpResponse();
        }

        [HttpPut("{listingId}/status/offmarket")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> MarkAsOffMarket(int listingId)
        {
            var result = await listingService.MarkAsOffMarket(listingId);
            return result.HttpResponse();
        }

        [HttpDelete("{listingId}/image/{imageId}")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> RemoveImage(int listingId, int imageId)
        {
            var result = await listingService.RemoveImage(listingId, imageId);
            return result.HttpResponse();
        }

        [HttpPut("{listingId}/image/{imageId}/primary")]
        [Authorize(AppPolicies.REQUIRE_BROKER_ROLE)]
        public async Task<IActionResult> SetPrimaryImage(int listingId, int imageId)
        {
            var result = await listingService.SetPrimaryImage(listingId, imageId);
            return result.HttpResponse();
        }
    }
}
