using System;
using System.Net.Http;
using BFF.Configurations;

namespace BFF.Clients
{
    public class QuestionHttpClient : HttpClient
    {
        public QuestionHttpClient(QuestionServiceConfiguration serviceConfiguration)
        {
            BaseAddress = new Uri(serviceConfiguration.BasePath);
            Timeout = serviceConfiguration.TimeoutSeconds;
            DefaultRequestHeaders.Add(serviceConfiguration.APIHeader, serviceConfiguration.APIKey);

        }
    }
}
