using System;
using Microsoft.Extensions.Configuration;

namespace BFF.Configurations
{
    public class AnswerServiceConfiguration
    {
        public AnswerServiceConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(AnswerServiceConfiguration));

            if (int.TryParse(section[nameof(TimeoutSeconds)], out var result))
            {
                TimeoutSeconds = TimeSpan.FromSeconds(result);
            }

            BasePath = section[nameof(BasePath)];
            APIHeader = section[nameof(APIHeader)];
            APIKey = section[nameof(APIKey)];

            ReadFromQuestion = $"{BasePath}/{section[nameof(ReadFromQuestion)]}";

        }

        public TimeSpan TimeoutSeconds { get; }

        public string APIHeader { get; }

        public string APIKey { get; }

        public string BasePath { get; }

        public string ReadFromQuestion { get; }

    }
}
