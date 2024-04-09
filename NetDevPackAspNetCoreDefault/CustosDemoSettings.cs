using System.Security.Claims;
using Bogus;
using Microsoft.OpenApi.Models;

namespace NetDevPackAspNetCoreDefault
{
    public static class FakeClaims
    {
        /// <summary>
        /// Generates the claim.
        /// </summary>
        /// <returns>A Faker.</returns>
        public static Faker<Claim> GenerateClaim()
        {
            return new Faker<Claim>().CustomInstantiator(f => new Claim(f.Internet.DomainName(), f.Lorem.Text()));
        }
    }

    /// <summary>
    /// The custom swagger.
    /// </summary>
    public static class CustomSwagger
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
             {
                 c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                 {
                     Description = "Bearer {token}",
                     Name = "Authorization",
                     Scheme = "Bearer",
                     BearerFormat = "JWT",
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.ApiKey
                 });

                 c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                 });
             });
        }
    }
}
