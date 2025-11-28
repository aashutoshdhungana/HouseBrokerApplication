using Xunit;
using Moq;
using HouseBrokerApplication.API.Controllers;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Application.DTOs.Responses;
using HouseBrokerApplication.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HouseBrokerApplication.Test.Controllers
{
    public class ListingControllerTests
    {
        private readonly Mock<IListingService> _listingServiceMock;
        private readonly ListingController _controller;

        public ListingControllerTests()
        {
            _listingServiceMock = new Mock<IListingService>();
            _controller = new ListingController(_listingServiceMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsExpectedResult()
        {
            var request = new CreateUpdateListingRequest();
            var expectedResult = Result<ListingResponse>.Success("Added successfully", new ListingResponse());
            _listingServiceMock.Setup(s => s.CreateAsync(request)).ReturnsAsync(expectedResult);

            var result = await _controller.Create(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsExpectedResult()
        {
            var request = new CreateUpdateListingRequest();
            var expectedResult = Result<ListingResponse>.Success("Updated successfully", new ListingResponse());
            _listingServiceMock.Setup(s => s.Update(1, request)).ReturnsAsync(expectedResult);

            var result = await _controller.Update(1, request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsExpectedResult()
        {
            var expectedResult = Result<string>.Success("Successful", string.Empty);
            _listingServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(expectedResult);

            var result = await _controller.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsExpectedResult()
        {
            var expectedResult = Result<ListingResponse>.Success("Successful", new ListingResponse());
            _listingServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(expectedResult);

            var result = await _controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task GetDetailedById_ReturnsExpectedResult()
        {
            var expectedResult = Result<DetailedListing>.Success("Successful", new DetailedListing());
            _listingServiceMock.Setup(s => s.GetDetailedByIdAsync(1)).ReturnsAsync(expectedResult);

            var result = await _controller.GetDetailedById(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task GetListings_ReturnsExpectedResult()
        {
            var filters = new ListingFilters();
            var expectedResult = Result<PaginationResponse<ListingResponse>>.Success("Success", new PaginationResponse<ListingResponse>());
            _listingServiceMock.Setup(s => s.GetListings(filters)).ReturnsAsync(expectedResult);

            var result = await _controller.GetListings(filters);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task UploadImage_ReturnsExpectedResult()
        {
            var expectedResult = Result<string>.Success("Successfull", "url");
            var fileMock = new Mock<Microsoft.AspNetCore.Http.IFormFile>();
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<System.IO.Stream>(), default)).Returns(Task.CompletedTask);
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            var request = new UploadImageRequest { File = fileMock.Object, IsPrimary = true };
            _listingServiceMock.Setup(s => s.UploadImage(1, It.IsAny<byte[]>(), "test.jpg", true)).ReturnsAsync(expectedResult);

            var result = await _controller.UploadImage(1, request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task AddUpdateOffer_ReturnsExpectedResult()
        {
            var offer = new AddUpdateOffer();
            var expectedResult = Result<string>.Success("Offer added/updated successfully.", string.Empty);
            _listingServiceMock.Setup(s => s.AddUpdateOffer(1, offer)).ReturnsAsync(expectedResult);

            var result = await _controller.AddUpdateOffer(1, offer);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task RemoveOffer_ReturnsExpectedResult()
        {
            var expectedResult = Result<string>.Success("Offer removed successfully.", string.Empty);
            _listingServiceMock.Setup(s => s.RemoveOffer(1, 2)).ReturnsAsync(expectedResult);

            var result = await _controller.RemoveOffer(1, 2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task AcceptOffer_ReturnsExpectedResult()
        {
            var expectedResult = Result<string>.Success("Offer accepted and listing marked as Sold.", string.Empty);
            _listingServiceMock.Setup(s => s.AcceptOffer(1, 2)).ReturnsAsync(expectedResult);

            var result = await _controller.AcceptOffer(1, 2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task MarkAsOffMarket_ReturnsExpectedResult()
        {
            var expectedResult = Result<string>.Success("Listing marked as off-market successfully.", string.Empty);
            _listingServiceMock.Setup(s => s.MarkAsOffMarket(1)).ReturnsAsync(expectedResult);

            var result = await _controller.MarkAsOffMarket(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task RemoveImage_ReturnsExpectedResult()
        {
            var expectedResult = Result<string>.Success("Image removed successfully.", string.Empty);
            _listingServiceMock.Setup(s => s.RemoveImage(1, 2)).ReturnsAsync(expectedResult);

            var result = await _controller.RemoveImage(1, 2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task SetPrimaryImage_ReturnsExpectedResult()
        {
            var expectedResult = Result<string>.Success("Primary image set successfully.", string.Empty);
            _listingServiceMock.Setup(s => s.SetPrimaryImage(1, 2)).ReturnsAsync(expectedResult);

            var result = await _controller.SetPrimaryImage(1, 2);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }
    }
}
