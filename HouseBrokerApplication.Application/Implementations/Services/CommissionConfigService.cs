using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Domain.Aggregates.GlobalConfig;
using HouseBrokerApplication.Domain.Base;

namespace HouseBrokerApplication.Application.Implementations.Services
{
    public class CommissionConfigService(IRepository<CommissionConfig> commissionRepository) : IComissionConfigService
    {
        public async Task<decimal> CalculateCommission(decimal price)
        {
            var specificationFilter = new BaseSpecification<CommissionConfig>(x => x.StartingPrice <= price &&
            x.EndingPrice >= price);
            var commissionSlab = await commissionRepository.GetSingleBySpecification(specificationFilter);
            if (commissionSlab == null)
            {
                if (price < 5000000) return 2 * price * 0.01m;
                else if (price >= 5000000 && price <= 10000000) return 1.75m * price * 0.01m;
                else return 1.5m * price * 0.01m;
            }
            else
                return commissionSlab.CommissionRate * price * 0.01m;
        }
    }
}
