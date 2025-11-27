namespace HouseBrokerApplication.Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
    }
}
