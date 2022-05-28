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

namespace Answers.Infrastructure.InternalServices.Questions
{
    public class QuestionService : IQuestionService
    {
        private readonly ILogger<QuestionService> _Logger;
        private readonly QuestionHttpClient _Client;

        public QuestionService(QuestionHttpClient client, ILogger<QuestionService> logger)
        {
            _Client = client;
            _Logger = logger;

        }
        
        public async Task<bool> Exist(Guid id, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _Client.GetAsync($"api/Question/{id}", cancellationToken);

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    _Logger.LogDebug("La pregunta de identificador {0} existe", id);
                    return true;
                }
                case HttpStatusCode.NotFound:
                {
                    _Logger.LogInformation("La pregunta de identificador {0} NO existe", id);
                    return false;
                }
                default:
                {
                    _Logger.LogError("Se ha detectado una respuesta no esperada por el endpoint {0}, el contenido es: {1}", 
                        $"api/Question/{id}", await httpResponse.Content.ReadAsStringAsync(CancellationToken.None));

                    return false;
                }
            }
        }

        public async Task<ReadOne.DTO?> ReadOneAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _Client.GetAsync($"api/Question/{id}", cancellationToken);

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    _Logger.LogInformation("La pregunta de identificador {0} existe", id);

                    var jsonResponse = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var dto = JsonConvert.DeserializeObject<ReadOne.DTO>(jsonResponse);

                    return dto;

                }
                case HttpStatusCode.NotFound:
                {
                    _Logger.LogWarning("La pregunta de identificador {0} NO existe", id);
                    return null;
                }
                default:
                {
                    _Logger.LogError("Se ha detectado una respuesta no esperada por el endpoint {0}, el código de estado recibido fue {1} y con contenido: {2}",
                        $"api/Question/{id}", httpResponse.StatusCode.ToString(), await httpResponse.Content.ReadAsStringAsync(CancellationToken.None));

                    return null;
                }
            }
        }
    }
}
