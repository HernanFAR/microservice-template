using Authentications.Application;
using Authentications.Application.Abstractions;
using Authentications.Application.Configurations;
using Authentications.Infrastructure.Abstractions;
using Authentications.Infrastructure.Abstractions.BackgroundTaskQueue;
using Authentications.Infrastructure.Abstractions.Email;
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

            // Abstracts
            serviceCollection.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            serviceCollection.AddScoped<IEmailSender, EmailSender>();
            serviceCollection.AddScoped<IRazorViewRenderService, RazorViewRenderService>();
            serviceCollection.AddScoped<ITokenGenerator, TokenGenerator>();

            // BackgroundTaskQueue
            serviceCollection.AddHostedService<QueuedHostedService>();
            serviceCollection.AddSingleton(provider => new BackgroundTaskConfiguration(provider.GetRequiredService<IConfiguration>()));

            // EmailSender
            serviceCollection.AddSingleton(provider => new SendGridConfiguration(provider.GetRequiredService<IConfiguration>()));

            // RazorViewRenderService
            serviceCollection.AddRazorPages();

        }
    }
}
