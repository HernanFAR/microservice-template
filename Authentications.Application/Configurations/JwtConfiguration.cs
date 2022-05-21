using System.Text;
using Microsoft.Extensions.Configuration;

namespace Authentications.Application.Configurations
{
    public class JwtConfiguration
    {
        public JwtConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection("JwtConfiguration");

            IssuerSigningKey = section["IssuerSigningKey"];
            Issuer = section["Issuer"];
            Audience = section["Audience"];

            if (int.TryParse(section["Duration"], out var result))
            {
                Duration = result;
            }

        }

        public JwtConfiguration(string issuerSigningKey, string issuer, string audience, int duration)
        {
            IssuerSigningKey = issuerSigningKey;
            Issuer = issuer;
            Audience = audience;
            Duration = duration;
        }

        public string IssuerSigningKey { get; }

        public string Issuer { get; }

        public string Audience { get; }

        public int Duration { get; }

        public byte[] IssuerBytes => Encoding.UTF8.GetBytes(IssuerSigningKey);

    }
}
