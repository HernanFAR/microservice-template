using System.Text;
using Microsoft.Extensions.Configuration;

namespace Questions.Application.Configurations
{
    public class JwtConfiguration
    {
        public JwtConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(JwtConfiguration));

            IssuerSigningKey = section[nameof(IssuerSigningKey)];
            Issuer = section[nameof(Issuer)];
            Audience = section[nameof(Audience)];

            if (int.TryParse(section[nameof(Duration)], out var result))
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
