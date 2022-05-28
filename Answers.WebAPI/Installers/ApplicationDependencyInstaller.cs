using System.Net.Mime;
using Answers.Application.Configurations;
using Answers.Application.InternalServices;
using Answers.Infrastructure.InternalServices.Questions;
using Answers.Infrastructure.InternalServices.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedKernel.Application.Abstractions;
using SharedKernel.Domain.Others;
using SharedKernel.Infrastructure.Application.Email;
using SharedKernel.Infrastructure.Others;
using SharedKernel.WebAPI.Interfaces;

namespace Answers.WebAPI.Installers
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
            
            serviceCollection.AddScoped<IEmailSender, EmailSender>();
            serviceCollection.AddSingleton(provider => new EmailConfiguration(provider.GetRequiredService<IConfiguration>()));
            serviceCollection.AddSingleton(provider => new SendGridConfiguration(provider.GetRequiredService<IConfiguration>()));

            // QuestionInternalService
            serviceCollection.AddScoped<QuestionHttpClient>();
            serviceCollection.AddScoped(provider => new QuestionServiceConfiguration(provider.GetRequiredService<IConfiguration>()));
            serviceCollection.AddScoped<IQuestionService, QuestionService>();

            // UserInternalService
            serviceCollection.AddScoped<UserHttpClient>();
            serviceCollection.AddScoped(provider => new UserServiceConfiguration(provider.GetRequiredService<IConfiguration>()));
            serviceCollection.AddScoped<IUserService, UserService>();

        }
    }
}
