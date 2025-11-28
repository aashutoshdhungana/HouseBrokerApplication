namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface IComissionConfigService
    {
        public Task<decimal> CalculateCommission(decimal price);
    }
}
