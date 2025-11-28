using FluentValidation;
using HouseBrokerApplication.API.Services;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.Implementations.Services;
using HouseBrokerApplication.Application.Validators;
using HouseBrokerApplication.Domain.Base;
using HouseBrokerApplication.Infrastructure.Implementations;
using HouseBrokerApplication.Infrastructure.Implementations.Services;

namespace HouseBrokerApplication.API.Configurations
{
    public static class ApplicationServiceConfig
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(RegisterBrokerValidator).Assembly);
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserIdentityService, UserIdentityService>();
            services.AddScoped<IListingService, ListingService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IComissionConfigService, CommissionConfigService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }
    }
}
