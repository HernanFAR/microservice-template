using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Authentications.Infrastructure.Abstractions.Dox
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
