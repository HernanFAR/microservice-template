using Answers.Domain.DataAccess;
using Answers.Domain.DataAccess.Repositories;
using Answers.EntityFramework;
using Answers.Infrastructure;
using Answers.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.Infrastructure.EntityFrameworkCore.Interceptors;
using SharedKernel.WebAPI.Interfaces;

namespace Answers.WebAPI.Installers
{
    public class DomainDependencyInstaller : IDependencyInstaller
    {
        public void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // ORM
            serviceCollection.AddDbContext<ApplicationDbContext>(
                (provider, options) =>
                {
                    var env = provider.GetRequiredService<IWebHostEnvironment>();

                    if (!env.IsProduction())
                    {
                        options = options.EnableSensitiveDataLogging();
                    }

                    options = options.UseNpgsql(configuration.GetConnectionString("Database"),
                            options => options.MigrationsHistoryTable("__EFMigrationHistory", DatabaseConstants.Schema))
                        .AddInterceptors(provider.GetServices<IInterceptor>());

                })
                .AddHttpContextAccessor();

            serviceCollection.AddScoped<IInterceptor, AggregateRootValidatorInterceptor>();
            serviceCollection.AddScoped<IInterceptor, EventInvokerInterceptor>();

            // Data Access
            serviceCollection.AddScoped<IAnswerUnitOfWork, AnswerUnitOfWork>();
            serviceCollection.AddScoped<IAnswerRepository, AnswerRepository>();
        }
    }
}
