using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedKernel.Domain.Others;
using SharedKernel.Infrastructure.Others;
using SharedKernel.WebAPI.Interfaces;

namespace Questions.WebAPI.Installers
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
            serviceCollection.AddValidatorsFromAssemblies(new[] { typeof(Application.Anchor).Assembly });

            // MediatR
            serviceCollection.AddMediatR(typeof(Application.Anchor).Assembly);
        }
    }
}
