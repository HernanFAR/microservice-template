using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SharedKernel.WebAPI.Configurations
{
    public class UseAPIKeyConfiguration
    {
        public UseAPIKeyConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(UseAPIKeyConfiguration));

            APIKey = section[nameof(APIKey)];
            APIHeader = section[nameof(APIHeader)];

        }

        public string APIHeader { get; }

        public string APIKey { get; }
    }
}
