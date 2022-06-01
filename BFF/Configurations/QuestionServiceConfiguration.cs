using System;
using Microsoft.Extensions.Configuration;

namespace BFF.Configurations
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

            BasePath = section[nameof(BasePath)];
            APIHeader = section[nameof(APIHeader)];
            APIKey = section[nameof(APIKey)];

            ReadOne = $"{BasePath}/{section[nameof(ReadOne)]}";

        }

        public TimeSpan TimeoutSeconds { get; }

        public string APIHeader { get; }

        public string APIKey { get; }

        public string BasePath { get; }

        public string ReadOne { get; }

    }
}
