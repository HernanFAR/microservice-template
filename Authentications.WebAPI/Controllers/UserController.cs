using System;
using System.Collections.Generic;
using Authentications.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Domain.Others;
using SharedKernel.WebAPI.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

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

        [SwaggerResponse(StatusCodes.Status201Created, "Se ha creado correctamente el recurso en el sistema.",
            typeof(CreateUser.DTO))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada.",
            typeof(IReadOnlyList<ValidationError>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [HttpPost("Register")]
        public async Task<ActionResult<CreateUser.DTO>> Register(
            [FromBody]
            CreateUser.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw BusinessException.UnProcessableEntityWithMessage("No se ha ingresado un body correctamente formateado o es uno invalido");
            }

            var response = await _Sender.Send(request, cancellationToken);

            // TODO: Ver como usar método Created
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [SwaggerOperation(Summary = "Inicia sesión con correo y contraseña, retornando un token JWT.")]

        [SwaggerResponse(StatusCodes.Status201Created,
            "Se ha creado iniciado correctamente la sesión.",
            typeof(Login.DTO))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "No se ha ingresado la contraseña correcta.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "No tienes permitido el inicio de sesión.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No se ha encontrado un usuario con ese correo.")]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada.",
            typeof(IReadOnlyList<ValidationError>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [HttpPost("Login")]
        public async Task<ActionResult<Login.DTO>> Login(
            [FromBody]
            Login.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw BusinessException.UnProcessableEntityWithMessage("No se ha ingresado un body correctamente formateado o es uno invalido");
            }

            var response = await _Sender.Send(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [SwaggerOperation(Summary = "Retorna información básica de un usuario, en base a su identificador.")]

        [SwaggerResponse(StatusCodes.Status200OK, "Se ha encontrado un usuario con ese identificador.",
            typeof(Read.DTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No se ha encontrado un usuario con ese identificador.")]

        [Produces(MediaTypeNames.Application.Json)]

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<Read.DTO>> Read(
            [FromQuery] Guid userId, CancellationToken cancellationToken)
        {
            var response = await _Sender.Send(new Read.Query(userId), cancellationToken);

            if (response is null) return NotFound();

            return Ok(response);
        }

    }
}
