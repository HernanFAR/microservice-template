using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Questions.Application.Features;
using SharedKernel.Domain.Others;
using SharedKernel.WebAPI.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Contiene funciones relacionadas a la gestión de preguntas")]
    public class QuestionController : ControllerBase
    {
        private readonly ISender _Sender;

        public QuestionController(ISender sender)
        {
            _Sender = sender;
        }

        [SwaggerOperation(Summary = "Retorna información de todos las preguntas.")]
        [SwaggerResponse(StatusCodes.Status200OK,
            "Se han leído y entregados correctamente el recurso.",
            typeof(ControllerResponse<IReadOnlyList<ReadAll.DTO>>))]

        [Produces(MediaTypeNames.Application.Json)]

        [HttpGet]
        public async Task<ActionResult<ControllerResponse<IReadOnlyList<ReadAll.DTO>>>> Get(CancellationToken cancellationToken)
        {
            var response = await _Sender.Send(new ReadAll.Query(), cancellationToken);

            return StatusCode(StatusCodes.Status200OK,
                ControllerResponse<IReadOnlyList<ReadAll.DTO>>.SuccessWith(response));
        }

        [SwaggerOperation(Summary = "Retorna información de una pregunta en base a su ID.")]
        [SwaggerResponse(StatusCodes.Status200OK,
            "Se ha leído y entregado correctamente el recurso.",
            typeof(ControllerResponse<ReadOne.DTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No se ha encontrado un recurso con ese identificador.",
            typeof(ControllerResponse<object>))]

        [Produces(MediaTypeNames.Application.Json)]

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ControllerResponse<ReadOne.DTO>>> Get(
            [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var response = await _Sender.Send(new ReadOne.Query(id), cancellationToken);

            if (response is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ControllerResponse<object>.FailureWith(
                    new ValidationError(nameof(id), "No se ha encontrado una pregunta con ese identificador")));
            }

            return StatusCode(StatusCodes.Status200OK,
                ControllerResponse<ReadOne.DTO>.SuccessWith(response));
        }

        [SwaggerOperation(Summary = "Crea una pregunta, en base a la información enviada en el body.")]
        [SwaggerResponse(StatusCodes.Status201Created,
            "Se ha creado correctamente el recurso en el sistema.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada.",
            typeof(ControllerResponse<object>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<ControllerResponse<object>>> Post(
            [FromBody]
            Create.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw BusinessException.UnProcessableEntityWithMessage("No se ha ingresado un body correctamente formateado o es uno invalido");
            }

            var response = await _Sender.Send(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created,
                ControllerResponse<object>.SuccessWith(response));
        }

        [SwaggerOperation(Summary = "Actualiza una pregunta, en base a la información enviada en el body.")]

        [SwaggerResponse(StatusCodes.Status200OK,
            "Se ha actualizado correctamente el recurso en el sistema.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No se ha encontrado un recurso con ese identificador.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable,
            "El identificador del ejemplo de la URL y del body no coinciden.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada en el body.",
            typeof(ControllerResponse<object>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ControllerResponse<object>>> Put(
            [FromRoute]
            Guid id,
            [FromBody]
            Update.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw BusinessException.UnProcessableEntityWithMessage("No se ha ingresado un body correctamente formateado o es uno invalido");
            }

            if (id != request.Id)
            {
                throw BusinessException.NotAcceptableWithMessage("El identificador del ejemplo de la URL y el body no son el mismo");
            }

            var response = await _Sender.Send(request, cancellationToken);

            return StatusCode(StatusCodes.Status200OK,
                ControllerResponse<object>.SuccessWith(response));
        }

        [SwaggerOperation(Summary = "Borra una pregunta, en base al identificador enviado por parametro.")]
        [SwaggerResponse(StatusCodes.Status200OK,
            "Se ha actualizado correctamente el recurso en el sistema.",
            typeof(ControllerResponse<object>))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No se ha encontrado un recurso con ese identificador.",
            typeof(ControllerResponse<object>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ControllerResponse<object>>> Delete(
            [FromRoute]
            Guid id, CancellationToken cancellationToken)
        {
            await _Sender.Send(new Delete.Command(id), cancellationToken);

            return StatusCode(StatusCodes.Status200OK,
                ControllerResponse<object>.EmptySuccess());
        }
    }
}
