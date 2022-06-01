using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BFF.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadToAnonymousObjectAsync<T>(this HttpContent @this, T @object, CancellationToken cancellationToken = default)
        {
            var stringResponse = await @this.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeAnonymousType(stringResponse, @object);
        }

        public static async Task<List<T>> ReadToAnonymousListAsync<T>(this HttpContent @this, T @object, CancellationToken cancellationToken = default)
        {
            var stringResponse = await @this.ReadAsStringAsync(cancellationToken);

            var objectList = new[] {@object}.ToList();

            return JsonConvert.DeserializeAnonymousType(stringResponse, objectList, new JsonSerializerSettings());
        }
    }
}
