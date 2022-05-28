using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Answers.Application.InternalServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedKernel.WebAPI.Responses;

namespace Answers.Infrastructure.InternalServices.Users
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _Logger;
        private readonly UserHttpClient _Client;

        public UserService(UserHttpClient client, ILogger<UserService> logger)
        {
            _Client = client;
            _Logger = logger;

        }

        public async Task<Read.DTO?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _Client.GetAsync($"api/User/{id}", cancellationToken);

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    _Logger.LogInformation("El usuario de identificador {0} existe", id);

                    var jsonResponse = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var dto = JsonConvert.DeserializeObject<Read.DTO>(jsonResponse);

                    return dto;

                }
                case HttpStatusCode.NotFound:
                {
                    _Logger.LogWarning("El usuario de identificador {0} NO existe", id);
                    return null;
                }
                default:
                {
                    _Logger.LogError("Se ha detectado una respuesta no esperada por el endpoint {0}, el código de estado recibido fue {1} y con contenido: {2}",
                        $"api/User/{id}", httpResponse.StatusCode.ToString(), await httpResponse.Content.ReadAsStringAsync(CancellationToken.None));

                    return null;
                }
            }
        }
    }
}
