using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedKernel.Domain.Others;
using SharedKernel.Infrastructure.MediatR.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.MediatR.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _Logger;
        private readonly IRequestInformation<TRequest> _RequestInformation;

        public LoggingBehavior(ILogger<TRequest> logger, IRequestInformation<TRequest> requestInformation)
        {
            _Logger = logger;
            _RequestInformation = requestInformation;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _Logger.LogInformation(
                "Correlativo: {0} - Hora del registro: {1} | Iniciando manejo de {2}, con los siguientes datos: {3}.",
                _RequestInformation.RequestId, DateTime.Now, typeof(TRequest).Name, JsonConvert.SerializeObject(request));

            try
            {
                var response = await next();

                _Logger.LogInformation("Correlativo: {0} - Hora del registro: {1} | Terminado manejo de {2}, respuesta recibida correctamente.",
                    _RequestInformation.RequestId, DateTime.Now, typeof(TRequest).Name);

                return response;
            }
            catch (ValidationException ex)
            {
                _Logger.LogWarning("Correlativo: {0} - Hora del registro: {1} | Error de validación arrojado mientras se manejaba {2}, los errores fueron los siguientes: {3}.",
                    _RequestInformation.RequestId, DateTime.Now, typeof(TRequest).Name, JsonConvert.SerializeObject(ex.Errors));

                throw;
            }
            catch (BusinessException ex)
            {
                _Logger.LogWarning("Correlativo: {0} - Hora del registro: {1} | Error de validación arrojado mientras se manejaba {2}, los errores fueron los siguientes: {3} y el código de estado fue: {4}.",
                    _RequestInformation.RequestId, DateTime.Now, typeof(TRequest).Name, ex.Message, ex.StatusCode);

                throw;
            }
            catch (Exception ex)
            {
                _Logger.LogError("Correlativo: {0} - Hora del registro: {1} | Excepción de tipo {2} arrojada mientras se manejaba {3} con propiedades: {4}.",
                    _RequestInformation.RequestId, DateTime.Now, ex.GetType().Name, typeof(TRequest).Name, JsonConvert.SerializeObject(ex));

                throw;
            }
        }
    }
}
