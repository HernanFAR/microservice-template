using Microsoft.Extensions.Configuration;

namespace SharedKernel.Infrastructure.Application.BackgroundTaskQueue
{
    public class BackgroundTaskConfiguration
    {
        public int Capacity { get; }

        public BackgroundTaskConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(BackgroundTaskConfiguration));

            if (int.TryParse(section[nameof(Capacity)], out var result))
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
