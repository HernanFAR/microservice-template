using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Answers.Infrastructure.InternalServices.Users
{
    public class UserHttpClient : HttpClient
    {
        public UserHttpClient(UserServiceConfiguration configuration)
        {
            BaseAddress = new Uri(configuration.Url);
            Timeout = configuration.TimeoutSeconds;
            DefaultRequestHeaders.Add(configuration.APIHeader, configuration.APIKey);

        }
    }
}
