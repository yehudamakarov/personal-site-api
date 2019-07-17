using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace PersonalSiteApi.StartupHelper
{
    public static class StartupAuthentication
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        var signingKeyBytes = Convert.FromBase64String(configuration["JWT_SIGNING_KEY"]);
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    }
                );
            return services;
        }
    }
}