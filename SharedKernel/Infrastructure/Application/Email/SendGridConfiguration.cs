using Microsoft.Extensions.Configuration;

namespace SharedKernel.Infrastructure.Application.Email
{
    public class SendGridConfiguration
    {
        public SendGridConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(SendGridConfiguration));

            APIKey = section[nameof(APIKey)];

        }

        public SendGridConfiguration(string apiKey)
        {
            APIKey = apiKey;

        }

        public string APIKey { get; }

    }
}
