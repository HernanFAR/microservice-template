using System.Text;
using Authentications.Application.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.WebAPI.Interfaces;

namespace Authentications.WebAPI.Installers
{
    public class SecurityDependencyInstaller : IDependencyInstaller
    {
        public void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    var provider = serviceCollection.BuildServiceProvider();
                    var jwtConfiguration = provider.GetRequiredService<JwtConfiguration>();

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = jwtConfiguration.Audience,
                        ValidIssuer = jwtConfiguration.Issuer,
                        RequireExpirationTime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtConfiguration.IssuerBytes)
                    };

                });

            serviceCollection.AddAuthorization();

            serviceCollection.AddSingleton<JwtConfiguration>();

        }
    }
}
