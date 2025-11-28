using Xunit;
using Moq;
using HouseBrokerApplication.Application.Implementations.Services;
using HouseBrokerApplication.Application.DTOs.Requests;
using HouseBrokerApplication.Application.DTOs.Responses;
using HouseBrokerApplication.Domain.Aggregates.Listing;
using HouseBrokerApplication.Domain.Aggregates.UserInfo;
using HouseBrokerApplication.Domain.Base;
using HouseBrokerApplication.Application.Abstractions.Services;
using FluentValidation;
using FluentValidation.Results;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;

namespace HouseBrokerApplication.Test.Services
{
    public class ListingServiceTests
    {
        private readonly Mock<IRepository<Listing>> _listingRepoMock = new();
        private readonly Mock<IRepository<UserInfo>> _userRepoMock = new();
        private readonly Mock<IValidator<CreateUpdateListingRequest>> _validatorMock = new();
        private readonly Mock<IFileService> _fileServiceMock = new();
        private readonly Mock<ICurrentUserService> _currentUserMock = new();
        private readonly Mock<IComissionConfigService> _commissionMock = new();
        private readonly ListingService _service;

        public ListingServiceTests()
        {
            _service = new ListingService(
                _listingRepoMock.Object,
                _userRepoMock.Object,
                _validatorMock.Object,
                _fileServiceMock.Object,
                _currentUserMock.Object,
                _commissionMock.Object
            );
        }

        private Listing CreateListing(int brokerId = 1)
        {
            return new Listing("t", "d", new Address("a","b","c","d","e"), 1, ListingType.House, "p", "e", brokerId);
        }

        [Fact]
        public async Task AcceptOffer_Scenarios()
        {
            // Unauthorized
            _currentUserMock.Setup(x => x.UserId).Returns(() => null);
            var r1 = await _service.AcceptOffer(1, 1);
            Assert.Equal(ResultType.Unathorized, r1.ResultType);

            // Listing null
            _currentUserMock.Setup(x => x.UserId).Returns(1);
            _listingRepoMock.Setup(x => x.GetSingleBySpecification(It.IsAny<ISpecification<Listing>>())).ReturnsAsync((Listing)null);
            var r2 = await _service.AcceptOffer(1, 1);
            Assert.Equal(ResultType.NotFound, r2.ResultType);

            // Not broker
            var listing = CreateListing(brokerId: 2);
            _listingRepoMock.Setup(x => x.GetSingleBySpecification(It.IsAny<ISpecification<Listing>>())).ReturnsAsync(listing);
            _currentUserMock.Setup(x => x.UserId).Returns(1);
            var r3 = await _service.AcceptOffer(1, 1);
            Assert.Equal(ResultType.Unathorized, r3.ResultType);

            // Offer missing
            listing = CreateListing(brokerId: 1);
            _listingRepoMock.Setup(x => x.GetSingleBySpecification(It.IsAny<ISpecification<Listing>>())).ReturnsAsync(listing);
            _currentUserMock.Setup(x => x.UserId).Returns(1);
            var r4 = await _service.AcceptOffer(1, 1);
            Assert.Equal(ResultType.NotFound, r4.ResultType);

            // Success
            var buyer = new UserInfo("b","l","p","e");
            typeof(UserInfo).GetProperty("Id")?.SetValue(buyer, 3);
            var offer = new Offer(listing, buyer, 100);
            typeof(Listing).GetField("_offers", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(listing, new List<Offer> { offer });
            _commissionMock.Setup(c => c.CalculateCommission(offer.OfferAmount)).ReturnsAsync(5);
            var uow = new Mock<IUnitOfWork>(); uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _listingRepoMock.Setup(x => x.UnitOfWork).Returns(uow.Object);
            _listingRepoMock.Setup(x => x.Update(listing));
            var r5 = await _service.AcceptOffer(1, offer.Id);
            Assert.Equal(ResultType.Success, r5.ResultType);
        }

        [Fact]
        public async Task AddUpdateOffer_Scenarios()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(() => null);
            var a1 = await _service.AddUpdateOffer(1, new AddUpdateOffer { OfferPrice = 10 });
            Assert.Equal(ResultType.Unathorized, a1.ResultType);

            _currentUserMock.Setup(x => x.UserId).Returns(2);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Listing)null);
            var a2 = await _service.AddUpdateOffer(1, new AddUpdateOffer { OfferPrice = 10 });
            Assert.Equal(ResultType.NotFound, a2.ResultType);

            var list = CreateListing(brokerId:1);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(list);
            _userRepoMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync((UserInfo)null);
            var a3 = await _service.AddUpdateOffer(1, new AddUpdateOffer { OfferPrice = 10 });
            Assert.Equal(ResultType.NotFound, a3.ResultType);

            var buyer = new UserInfo("f","l","p","e"); typeof(UserInfo).GetProperty("Id")?.SetValue(buyer, 1);
            _userRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(buyer);
            _currentUserMock.Setup(x => x.UserId).Returns(1);
            var a4 = await _service.AddUpdateOffer(1, new AddUpdateOffer { OfferPrice = 10 });
            Assert.Equal(ResultType.Error, a4.ResultType);

            typeof(UserInfo).GetProperty("Id")?.SetValue(buyer, 2);
            _userRepoMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(buyer);
            _currentUserMock.Setup(x => x.UserId).Returns(2);
            var uow = new Mock<IUnitOfWork>(); uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _listingRepoMock.Setup(x => x.UnitOfWork).Returns(uow.Object);
            var a5 = await _service.AddUpdateOffer(1, new AddUpdateOffer { OfferPrice = 100 });
            Assert.Equal(ResultType.Success, a5.ResultType);
        }

        [Fact]
        public async Task Create_Update_Delete_Scenarios()
        {
            var req = new CreateUpdateListingRequest { Title = "t", Description = "d", Address = new Address("a","b","c","d","e"), Price = 1, PropertyType = ListingType.House, ContactPhone = "p", ContactEmail = "e" };
            _validatorMock.Setup(v => v.Validate(req)).Returns(new ValidationResult());

            _currentUserMock.Setup(x => x.UserId).Returns(() => null);
            var c1 = await _service.CreateAsync(req);
            Assert.Equal(ResultType.Unathorized, c1.ResultType);

            _currentUserMock.Setup(x => x.UserId).Returns(1);
            var uow = new Mock<IUnitOfWork>(); uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _listingRepoMock.Setup(x => x.UnitOfWork).Returns(uow.Object);
            var c2 = await _service.CreateAsync(req);
            Assert.Equal(ResultType.Success, c2.ResultType);

            _currentUserMock.Setup(x => x.UserId).Returns(() => null);
            var up1 = await _service.Update(1, req);
            Assert.Equal(ResultType.Unathorized, up1.ResultType);

            _currentUserMock.Setup(x => x.UserId).Returns(1);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Listing)null);
            var up2 = await _service.Update(1, req);
            Assert.Equal(ResultType.NotFound, up2.ResultType);

            _currentUserMock.Setup(x => x.UserId).Returns(() => null);
            var d1 = await _service.DeleteAsync(1);
            Assert.Equal(ResultType.Unathorized, d1.ResultType);

            _currentUserMock.Setup(x => x.UserId).Returns(1);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Listing)null);
            var d2 = await _service.DeleteAsync(1);
            Assert.Equal(ResultType.NotFound, d2.ResultType);

            var listing = CreateListing(brokerId:1);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(listing);
            var uow2 = new Mock<IUnitOfWork>(); uow2.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _listingRepoMock.Setup(x => x.UnitOfWork).Returns(uow2.Object);
            var d3 = await _service.DeleteAsync(1);
            Assert.Equal(ResultType.Success, d3.ResultType);
        }

        [Fact]
        public async Task GetAndListingQueries()
        {
            _listingRepoMock.Setup(x => x.GetSingleBySpecification(It.IsAny<ISpecification<Listing>>())).ReturnsAsync((Listing)null);
            var r1 = await _service.GetByIdAsync(1);
            Assert.Equal(ResultType.NotFound, r1.ResultType);

            var listing = CreateListing(1);
            _listingRepoMock.Setup(x => x.GetSingleBySpecification(It.IsAny<ISpecification<Listing>>())).ReturnsAsync(listing);
            var r2 = await _service.GetByIdAsync(1);
            Assert.Equal(ResultType.Success, r2.ResultType);

            _listingRepoMock.Setup(x => x.GetBySpecification(It.IsAny<ISpecification<Listing>>())).ReturnsAsync(new List<Listing> { listing });
            var filters = new ListingFilters();
            var r3 = await _service.GetListings(filters);
            Assert.Equal(ResultType.Success, r3.ResultType);

            var r4 = await _service.GetBrokerListing(1, filters);
            Assert.Equal(ResultType.Success, r4.ResultType);
        }

        [Fact]
        public async Task MarkAsOffMarket_Scenarios()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(() => null);
            var r1 = await _service.MarkAsOffMarket(1);
            Assert.Equal(ResultType.Unathorized, r1.ResultType);

            _currentUserMock.Setup(x => x.UserId).Returns(1);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Listing)null);
            var r2 = await _service.MarkAsOffMarket(1);
            Assert.Equal(ResultType.NotFound, r2.ResultType);

            var listing = CreateListing(brokerId:1);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(listing);
            var uow = new Mock<IUnitOfWork>(); uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _listingRepoMock.Setup(x => x.UnitOfWork).Returns(uow.Object);
            var r3 = await _service.MarkAsOffMarket(1);
            Assert.Equal(ResultType.Success, r3.ResultType);
        }

        [Fact]
        public async Task UpdateAndUploadImage()
        {
            var req = new CreateUpdateListingRequest { Title = "t", Description = "d", Address = new Address("a","b","c","d","e"), Price = 1, PropertyType = ListingType.House, ContactPhone = "p", ContactEmail = "e" };
            _validatorMock.Setup(v => v.Validate(req)).Returns(new ValidationResult());
            _currentUserMock.Setup(x => x.UserId).Returns(1);
            var listing = CreateListing(brokerId:1);
            _listingRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(listing);
            var uow = new Mock<IUnitOfWork>(); uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _listingRepoMock.Setup(x => x.UnitOfWork).Returns(uow.Object);
            var up = await _service.Update(1, req);
            Assert.Equal(ResultType.Success, up.ResultType);

            var invalid = await _service.UploadImage(1, new byte[0], "file.txt", false);
            Assert.Equal(ResultType.Error, invalid.ResultType);

            _fileServiceMock.Setup(f => f.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync(Result<HouseBrokerApplication.Domain.Aggregates.FileInfo.FileInfo>.Success("ok", new HouseBrokerApplication.Domain.Aggregates.FileInfo.FileInfo("n","s","u")));
            _listingRepoMock.Setup(x => x.GetSingleBySpecification(It.IsAny<ISpecification<Listing>>())).ReturnsAsync(listing);
            var uow2 = new Mock<IUnitOfWork>(); uow2.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _listingRepoMock.Setup(x => x.UnitOfWork).Returns(uow2.Object);
            var valid = await _service.UploadImage(1, new byte[1], "image.jpg", true);
            Assert.Equal(ResultType.Success, valid.ResultType);
        }
    }
}
