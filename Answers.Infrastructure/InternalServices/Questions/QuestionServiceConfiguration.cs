using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Answers.Infrastructure.InternalServices.Questions
{
    public class QuestionServiceConfiguration
    {
        public QuestionServiceConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(QuestionServiceConfiguration));

            if (int.TryParse(section[nameof(TimeoutSeconds)], out var result))
            {
                TimeoutSeconds = TimeSpan.FromSeconds(result);
            }

            Url = section[nameof(Url)];
            APIHeader = section[nameof(APIHeader)];
            APIKey = section[nameof(APIKey)];

        }

        public QuestionServiceConfiguration(TimeSpan timeoutSeconds, string url, string apiKey, string apiHeader)
        {
            TimeoutSeconds = timeoutSeconds;
            Url = url;
            APIKey = apiKey;
            APIHeader = apiHeader;
        }

        public TimeSpan TimeoutSeconds { get; }

        public string Url { get; }

        public string APIHeader { get; }

        public string APIKey { get; }

    }
}
