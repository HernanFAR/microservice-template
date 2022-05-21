using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;
using Microsoft.AspNetCore.Routing.Tree;
using Newtonsoft.Json;

namespace Authentications.Infrastructure.Abstractions.Dox
{
    public class IpStackDoxer : IDoxer
    {
        private readonly IpStackConfiguration _Configuration;

        public IpStackDoxer(IpStackConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public async Task<DoxInfo?> DoxIpAsync(string ip, CancellationToken cancellationToken = default)
        {
            var url = $"http://api.ipstack.com/{ip}?access_key={_Configuration.APIKey}&language=es";

            using var client = new HttpClient();

            var response = await client.GetAsync(url, cancellationToken);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);

            dynamic? @object = JsonConvert.DeserializeObject(jsonContent);

            if (@object == null || @object?.error is not null)
            {
                return null;
            }

            return new DoxInfo(@object!.ip, @object.continent_name, @object.region_name, @object.city,
                @object.latitude, @object.longitude);
        }
    }
}
