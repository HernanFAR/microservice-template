using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Answers.Infrastructure.InternalServices.Questions
{
    public class QuestionHttpClient : HttpClient
    {
        public QuestionHttpClient(QuestionServiceConfiguration configuration)
        {
            BaseAddress = new Uri(configuration.Url);
            Timeout = configuration.TimeoutSeconds;
            DefaultRequestHeaders.Add(configuration.APIHeader, configuration.APIKey);

        }
    }
}
