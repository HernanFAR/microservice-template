using Microsoft.Extensions.Configuration;

namespace SharedKernel.Infrastructure.Application.BackgroundTaskQueue
{
    public class BackgroundTaskConfiguration
    {
        public int Capacity { get; }

        public BackgroundTaskConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection("TareasSegundoPlano");

            if (int.TryParse(section["Capacidad"], out var result))
            {
                Capacity = result;
            }
        }

        public BackgroundTaskConfiguration(int capacity)
        {
            Capacity = capacity;
        }
    }
}
