using Authentications.Application;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedKernel.Domain.Others;
using SharedKernel.Infrastructure.Others;
using SharedKernel.WebAPI.Interfaces;

namespace Authentications.WebAPI.Installers
{
    public class ApplicationDependencyInstaller : IDependencyInstaller
    {
        public void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Servicios
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            serviceCollection.AddScoped<ITimeProvider, TimeProvider>();

            // Validadores
            serviceCollection.AddValidatorsFromAssemblies(new[] { typeof(Anchor).Assembly });

            // MediatR
            serviceCollection.AddMediatR(typeof(Anchor).Assembly);
        }
    }
}
