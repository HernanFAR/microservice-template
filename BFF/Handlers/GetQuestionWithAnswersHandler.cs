using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BFF.Clients;
using BFF.Configurations;
using BFF.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;

namespace BFF.Handlers
{
    public class GetQuestionWithAnswersHandler : DelegatingHandler
    {
        private readonly AnswerHttpClient _AnswerClient;
        private readonly AnswerServiceConfiguration _Configuration;
        private readonly ILogger<GetQuestionWithAnswersHandler> _Logger;

        public GetQuestionWithAnswersHandler(AnswerHttpClient answerClient, AnswerServiceConfiguration configuration,
            ILogger<GetQuestionWithAnswersHandler> logger)
        {
            _AnswerClient = answerClient;
            _Configuration = configuration;
            _Logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var questionTask = base.SendAsync(request, cancellationToken);

            var query = request.RequestUri;

            if (query is null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var questionId = query.Segments.Last();

            var readFromQuestion = string.Format(_Configuration.ReadFromQuestion, questionId);
            var answerTask = _AnswerClient.GetAsync(readFromQuestion, cancellationToken);

            await Task.WhenAll(questionTask, answerTask);

            var questionResponse = await questionTask;

            switch (questionResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    _Logger.LogDebug("Se ha recibido un OK (200) por el endpoint {0}, la pregunta de id {1} SÍ existe",
                        query.ToString(), questionId);

                    break;

                case HttpStatusCode.NotFound:
                    _Logger.LogWarning("Se ha recibido un NotFound (404) por el endpoint {0}, la pregunta de id {1} NO existe", 
                        query.ToString(), questionId);

                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                default:
                    _Logger.LogError("Se ha detectado una respuesta no esperada por el endpoint {0}, el contenido es: {1}",
                        query.ToString(), await questionResponse.Content.ReadAsStringAsync(CancellationToken.None));

                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);

            }

            var answersResponse = await answerTask;

            switch (answersResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    _Logger.LogDebug("Se ha recibido un OK (200) por el endpoint {0}, se han obtenido las respuestas", 
                        readFromQuestion);

                    break;

                default:
                    _Logger.LogError("Se ha detectado una respuesta no esperada por el endpoint {0}, el contenido es: {1}",
                        readFromQuestion, await answersResponse.Content.ReadAsStringAsync(CancellationToken.None));

                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            var answers = await answersResponse.Content
                .ReadToAnonymousListAsync(new
                {
                    Id = Guid.Empty,
                    Name = string.Empty,
                    QuestionId = Guid.Empty,
                    Created = default(DateTime),
                    Updated = default(DateTime)

                }, cancellationToken);

            var question = await questionResponse.Content
                .ReadToAnonymousObjectAsync(new
                {
                    Id = Guid.Empty,
                    Name = string.Empty
                }, cancellationToken);

            var response = new
            {
                question.Id,
                question.Name,
                Answers = answers
            };

            return response.SerializeAsJsonToHttpMessage(HttpStatusCode.OK);
        }
    }
}
