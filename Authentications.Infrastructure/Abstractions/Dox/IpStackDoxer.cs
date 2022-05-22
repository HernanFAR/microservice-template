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

            var @object = JsonConvert.DeserializeObject<IpStackResponse>(jsonContent);

            if (@object is null or { Error: not null } or { Continent_Name: null }) 
            {
                return null;
            }

            return new DoxInfo(@object.Ip, @object.Continent_Name!, @object.Region_Name, @object.City,
                @object.Latitude.GetValueOrDefault(), @object.Longitude.GetValueOrDefault());
        }

        private record IpStackResponse(string Ip, string Continent_Name, string Region_Name, string City, long? Latitude, long? Longitude,
            object Error);
    }
}
