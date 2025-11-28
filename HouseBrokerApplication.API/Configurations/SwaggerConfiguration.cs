//using Microsoft.OpenApi;
//using Microsoft.OpenApi.;
//namespace HouseBrokerApplication.API.Configurations
//{
//    public static class SwaggerConfiguration
//    {
//        public static IServiceCollection ConfigureSwagger(this IServiceCollection services) 
//        {
//            services.AddEndpointsApiExplorer();
//            services.AddSwaggerGen(options =>
//            {
//                options.SwaggerDoc("v1", new() { Title = "House Broker API", Version = "v1" });

//                // Add JWT Authentication to Swagger
//                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    Name = "Authorization",
//                    Type = SecuritySchemeType.Http,
//                    Scheme = "Bearer",
//                    BearerFormat = "JWT",
//                    In = ParameterLocation.Header,
//                    Description = "Enter 'Bearer' followed by a space and JWT token."
//                });

//                options.AddSecurityRequirement((openAPiDoc) =>
//                    new OpenApiSecurityRequirement()
//                   .Add(new OpenApiSecuritySchemeReference("", new OpenApiDocument()
//                   {

//                   })
//                ));
//            });
//        }
//    }
//}
