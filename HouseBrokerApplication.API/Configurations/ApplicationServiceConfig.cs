using FluentValidation;
using HouseBrokerApplication.API.Services;
using HouseBrokerApplication.Application.Abstractions.Services;
using HouseBrokerApplication.Application.Validators;

namespace HouseBrokerApplication.API.Configurations
{
    public static class ApplicationServiceConfig
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(typeof(RegisterBrokerValidator).Assembly);
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
