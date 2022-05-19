using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.MediatR;
using SharedKernel.Infrastructure.MediatR.Behaviors;
using SharedKernel.Infrastructure.MediatR.Interfaces;
using SharedKernel.WebAPI.Interfaces;

namespace Questions.WebAPI.Installers
{
    public class PipelineBehaviorDependencyInstaller : IDependencyInstaller
    {
        public void InstallDependencies(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped(typeof(IRequestInformation<>), typeof(RequestInformation<>));
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}
