using Microsoft.Extensions.Configuration;

namespace SharedKernel.WebAPI.Configurations
{
    public class SwaggerAuthenticationConfiguration
    {
        public SwaggerAuthenticationConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(SwaggerAuthenticationConfiguration));

            Username = section[nameof(Username)];
            Password = section[nameof(Password)];
        }

        public SwaggerAuthenticationConfiguration(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }

        public string Password { get; }
    }
}
