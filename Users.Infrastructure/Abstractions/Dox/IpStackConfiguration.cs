using Microsoft.Extensions.Configuration;

namespace Users.Infrastructure.Abstractions.Dox
{
    public class IpStackConfiguration
    {
        public IpStackConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(IpStackConfiguration));

            APIKey = section[nameof(APIKey)];

        }

        public IpStackConfiguration(string apiKey)
        {
            APIKey = apiKey;
        }

        public string APIKey { get; }

    }
}
