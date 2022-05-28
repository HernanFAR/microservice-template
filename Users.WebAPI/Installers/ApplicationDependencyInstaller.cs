using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedKernel.Application.Abstractions;
using SharedKernel.Domain.Others;
using SharedKernel.Infrastructure.Application;
using SharedKernel.Infrastructure.Application.BackgroundTaskQueue;
using SharedKernel.Infrastructure.Application.Email;
using SharedKernel.Infrastructure.Others;
using SharedKernel.WebAPI.Interfaces;
using Users.Application;
using Users.Application.Abstractions;
using Users.Application.Configurations;
using Users.Infrastructure.Abstractions;
using Users.Infrastructure.Abstractions.Dox;

namespace Users.WebAPI.Installers
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
            serviceCollection.AddScoped<ITokenGenerator, TokenGenerator>();
            serviceCollection.AddScoped<IDoxer, IpStackDoxer>();

            // BackgroundTaskQueue
            serviceCollection.AddHostedService<QueuedHostedService>();
            serviceCollection.AddSingleton(provider => new BackgroundTaskConfiguration(provider.GetRequiredService<IConfiguration>()));

            // EmailSender
            serviceCollection.AddSingleton(provider => new EmailConfiguration(provider.GetRequiredService<IConfiguration>()));
            serviceCollection.AddSingleton(provider => new SendGridConfiguration(provider.GetRequiredService<IConfiguration>()));

            // RazorViewRenderService
            serviceCollection.AddRazorPages();

            // IpStackDoxer
            serviceCollection.AddSingleton(provider => new IpStackConfiguration(provider.GetRequiredService<IConfiguration>()));

        }
    }
}
