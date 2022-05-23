using Microsoft.Extensions.Configuration;

namespace Authentications.Application.Configurations
{
    public class EmailConfiguration
    {
        public EmailConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(EmailConfiguration));

            From = section[nameof(From)];
            FromName = section[nameof(FromName)] ?? section[nameof(From)];
        }

        public EmailConfiguration(string from, string? fromName = null)
        {
            From = from;
            FromName = fromName ?? from;
        }

        public string From { get; }

        public string FromName { get; }
    }
}
