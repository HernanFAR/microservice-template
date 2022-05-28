using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Answers.Infrastructure.InternalServices.Users
{
    public class UserServiceConfiguration
    {
        public UserServiceConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(UserServiceConfiguration));

            if (int.TryParse(section[nameof(TimeoutSeconds)], out var result))
            {
                TimeoutSeconds = TimeSpan.FromSeconds(result);
            }

            Url = section[nameof(Url)];

        }

        public UserServiceConfiguration(TimeSpan timeoutSeconds, string url)
        {
            TimeoutSeconds = timeoutSeconds;
            Url = url;
        }

        public string Url { get; }

        public TimeSpan TimeoutSeconds { get; }
    }
}
