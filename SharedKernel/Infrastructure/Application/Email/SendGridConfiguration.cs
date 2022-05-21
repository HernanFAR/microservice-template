using Microsoft.Extensions.Configuration;

namespace SharedKernel.Infrastructure.Application.Email
{
    public class SendGridConfiguration
    {
        public SendGridConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection("SendGrid");

            APIKey = section["APIKey"];
            From = section["From"];
            FromName = section["FromName"] ?? section["From"];

        }

        public SendGridConfiguration(string apiKey, string from, string? fromName = null)
        {
            APIKey = apiKey;
            From = from;
            FromName = fromName ?? from;
        }

        public string APIKey { get; }

        public string From { get; }

        public string FromName { get; }

    }
}
