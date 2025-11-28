using Xunit;
using Moq;
using HouseBrokerApplication.Application.Implementations.Services;
using HouseBrokerApplication.Domain.Aggregates.GlobalConfig;
using HouseBrokerApplication.Domain.Base;
using System.Threading.Tasks;

namespace HouseBrokerApplication.Test.Services
{
    public class CommissionConfigServiceTests
    {
        [Theory]
        [InlineData(4000000.00, 2.00)]
        [InlineData(5000000.00, 1.75)]
        [InlineData(10000000.00, 1.75)]
        [InlineData(15000000.00, 1.5)]
        public async Task CalculateCommission_FallbackRates_WhenNoSlab(decimal price, decimal expectedRate)
        {
            var repoMock = new Mock<IRepository<CommissionConfig>>();
            repoMock.Setup(r => r.GetSingleBySpecification(It.IsAny<ISpecification<CommissionConfig>>()))
                    .ReturnsAsync((CommissionConfig?)null);

            var service = new CommissionConfigService(repoMock.Object);

            var rate = await service.CalculateCommission(price);

            Assert.Equal(expectedRate, rate);
        }

        [Fact]
        public async Task CalculateCommission_ReturnsSlabRate_WhenSlabExists()
        {
            var slab = new CommissionConfig();
            typeof(CommissionConfig).GetProperty("StartingPrice")!.SetValue(slab, 0m);
            typeof(CommissionConfig).GetProperty("EndingPrice")!.SetValue(slab, decimal.MaxValue);
            typeof(CommissionConfig).GetProperty("CommissionRate")!.SetValue(slab, 3.5m);

            var repoMock = new Mock<IRepository<CommissionConfig>>();
            repoMock.Setup(r => r.GetSingleBySpecification(It.IsAny<ISpecification<CommissionConfig>>()))
                    .ReturnsAsync(slab);

            var service = new CommissionConfigService(repoMock.Object);

            var rate = await service.CalculateCommission(1_000_000.00m);

            Assert.Equal(3.5m, rate);
        }
    }
}
