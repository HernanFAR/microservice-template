using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Questions.Domain.DataAccess;
using Questions.Domain.DataAccess.Repositories;
using Questions.EntityFramework;
using Questions.Infrastructure;
using Questions.Infrastructure.DataAccess;
using SharedKernel.WebAPI.Interfaces;

namespace Questions.WebAPI.Installers
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

                });

            // Data Access
            serviceCollection.AddScoped<IQuestionUnitOfWork, QuestionUnitOfWork>();
            serviceCollection.AddScoped<IQuestionRepository, QuestionRepository>();
        }
    }
}
