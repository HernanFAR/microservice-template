using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Newtonsoft.Json;

namespace BFF.Extensions
{
    public static class ObjectExtensions
    {
        public static HttpResponseMessage SerializeAsJsonToHttpMessage<T>(this T @this, HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(@this))
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);

            return response;
        }
    }
}
