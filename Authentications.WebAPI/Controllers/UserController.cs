using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Domain.Others;
using SharedKernel.WebAPI.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Authentications.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Contiene funciones relacionadas a la gestión de preguntas")]
    public class UserController : ControllerBase
    {
        private readonly ISender _Sender;

        public UserController(ISender sender)
        {
            _Sender = sender;
        }

        [SwaggerOperation(Summary = "Crea un usuario, en base a la información enviada en el body y envía un correo de bienvenida.")]

        [SwaggerResponse(StatusCodes.Status201Created,
            "Se ha creado correctamente el recurso en el sistema.",
            typeof(ControllerResponse<CreateUser.DTO>))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada.",
            typeof(ControllerResponse<object>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [HttpPost("Register")]
        public async Task<ActionResult<ControllerResponse<CreateUser.DTO>>> Register(
            [FromBody]
            CreateUser.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw BusinessException.UnProcessableEntityWithMessage("No se ha ingresado un body correctamente formateado o es uno invalido");
            }

            var response = await _Sender.Send(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created,
                ControllerResponse<CreateUser.DTO>.SuccessWith(response));
        }

        [SwaggerOperation(Summary = "Inicia sesión con correo y contraseña, retornando un token JWT.")]
        [SwaggerResponse(StatusCodes.Status201Created,
            "Se ha creado iniciado correctamente la sesión.",
            typeof(ControllerResponse<Login.DTO>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized,
            "No se ha ingresado la contraseña correcta.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status403Forbidden,
            "No tienes permitido el inicio de sesión.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No se ha encontrado un usuario con ese correo.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada.",
            typeof(ControllerResponse<object>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [HttpPost("Login")]
        public async Task<ActionResult<ControllerResponse<Login.DTO>>> Login(
            [FromBody]
            Login.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw BusinessException.UnProcessableEntityWithMessage("No se ha ingresado un body correctamente formateado o es uno invalido");
            }

            var response = await _Sender.Send(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created,
                ControllerResponse<Login.DTO>.SuccessWith(response));
        }
    }
}
