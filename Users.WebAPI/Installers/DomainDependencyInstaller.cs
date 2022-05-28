using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.Infrastructure.EntityFrameworkCore.Interceptors;
using SharedKernel.WebAPI.Interfaces;
using Users.Domain.DataAccess;
using Users.Domain.Entities;
using Users.Domain.Entities.Users;
using Users.EntityFramework;
using Users.EntityFramework.Identity;

namespace Users.WebAPI.Installers
{
    public class DomainDependencyInstaller : IDependencyInstaller
    {
        public void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<ApplicationDbContext>(ContextOptionsBuilder)
                .AddIdentity<ApplicationUser, ApplicationRole>(IdentityConfiguration.ConfigureIdentity)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<SpanishIdentityErrorDescriber>();

            serviceCollection.AddScoped<IInterceptor, AggregateRootValidatorInterceptor>();
            serviceCollection.AddScoped<IInterceptor, EventInvokerInterceptor>();

        }

        private void ContextOptionsBuilder(IServiceProvider provider, DbContextOptionsBuilder options)
        {
            var env = provider.GetRequiredService<IWebHostEnvironment>();

            if (!env.IsProduction())
            {
                options = options.EnableSensitiveDataLogging();
            }

            var configuration = provider.GetRequiredService<IConfiguration>();

            options.UseNpgsql(configuration.GetConnectionString("Database"),
                    options => options.MigrationsHistoryTable("__EFMigrationHistory", DatabaseConstants.Schema))
                .AddInterceptors(provider.GetServices<IInterceptor>());
            
        }
    }
}
