using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.WebAPI.Interfaces
{
    public interface IDependencyInstaller
    {
        void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration);
    }
}
