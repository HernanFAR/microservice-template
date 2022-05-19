using Microsoft.Extensions.Configuration;
using SharedKernel.WebAPI.Interfaces;
using System;
using System.Linq;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InstallApplicationDependenciesFrom<TAnchor>(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(TAnchor).Assembly.ExportedTypes
                .Where(e => typeof(IDependencyInstaller).IsAssignableFrom(e))
                .Where(e => !e.IsAbstract && !e.IsInterface)
                .Select(Activator.CreateInstance)
                .Cast<IDependencyInstaller>()
                .ToList();

            foreach (var installer in installers)
            {
                installer.InstallDependencies(services, configuration);
            }

            return services;
        }
    }
}
