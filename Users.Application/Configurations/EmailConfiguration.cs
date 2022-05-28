using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Users.Application.Configurations
{
    public class EmailConfiguration
    {
        public EmailConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(EmailConfiguration));

            From = section[nameof(From)];
            FromName = section[nameof(FromName)] ?? section[nameof(From)];
            BCCs = section.GetSection(nameof(BCCs)).Get<List<string>>();

        }

        public EmailConfiguration(string from, string? fromName = null, IReadOnlyList<string>? bccs = null)
        {
            From = from;
            FromName = fromName ?? from;
            BCCs = bccs;
        }

        public IReadOnlyList<string>? BCCs { get; }

        public string From { get; }

        public string FromName { get; }
    }
}
