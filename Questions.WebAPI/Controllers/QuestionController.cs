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
using SharedKernel.WebAPI.Filters;

namespace Questions.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Contiene funciones relacionadas a la gestión de preguntas")]
    [UseAPIKey]
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
            typeof(IReadOnlyList<ReadAll.DTO>))]

        [Produces(MediaTypeNames.Application.Json)]

        [HttpGet(Name = "GetAllQuestion")]
        public async Task<ActionResult<IReadOnlyList<ReadAll.DTO>>> ReadAllAsync(CancellationToken cancellationToken)
        {
            var response = await _Sender.Send(new ReadAll.Query(), cancellationToken);

            return Ok(response);
        }

        [SwaggerOperation(Summary = "Retorna información de una pregunta en base a su ID.")]
        [SwaggerResponse(StatusCodes.Status200OK,
            "Se ha leído y entregado correctamente el recurso.",
            typeof(ReadOne.DTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No se ha encontrado un recurso con ese identificador.")]

        [Produces(MediaTypeNames.Application.Json)]

        [HttpGet("{id:guid}", Name = "GetQuestion")]
        public async Task<ActionResult<ReadOne.DTO>> ReadOneAsync(
            [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var response = await _Sender.Send(new ReadOne.Query(id), cancellationToken);

            if (response is null) return NotFound();

            return Ok(response);
        }

        [SwaggerOperation(Summary = "Crea una pregunta, en base a la información enviada en el body.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Se ha creado correctamente el recurso en el sistema.",
            typeof(Create.DTO))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada.",
            typeof(IReadOnlyList<ValidationError>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(Name = "CreateQuestion")]
        public async Task<ActionResult<Create.DTO>> CreateAsync(
            [FromBody]
            Create.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return UnprocessableEntity(new List<ValidationError>
                {
                    new("body", "No se ha ingresado un body correctamente formateado o es uno invalido")
                });
            }

            var response = await _Sender.Send(request, cancellationToken);

            return CreatedAtRoute("GetQuestion", new { id = response.Id }, response);
        }

        [SwaggerOperation(Summary = "Actualiza una pregunta, en base a la información enviada en el body.")]

        [SwaggerResponse(StatusCodes.Status200OK,
            "Se ha actualizado correctamente el recurso en el sistema.",
            typeof(Update.DTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No se ha encontrado un recurso con ese identificador.")]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable,
            "El identificador del ejemplo de la URL y del body no coinciden.")]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity,
            "Se han encontrado errores de validación en la información enviada en el body.",
            typeof(IReadOnlyList<ValidationError>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id:guid}", Name = "UpdateQuestion")]
        public async Task<ActionResult<Update.DTO>> UpdateAsync(
            [FromRoute]
            Guid id,
            [FromBody]
            Update.Command? request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return UnprocessableEntity(new List<ValidationError>
                {
                    new("body", "No se ha ingresado un body correctamente formateado o es uno invalido")
                });
            }

            if (id != request.Id)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            var response = await _Sender.Send(request, cancellationToken);

            return Ok(response);
        }

        [SwaggerOperation(Summary = "Borra una pregunta, en base al identificador enviado por parametro.")]
        [SwaggerResponse(StatusCodes.Status200OK,
            "Se ha actualizado correctamente el recurso en el sistema.")]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No se ha encontrado un recurso con ese identificador.")]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id:guid}", Name = "DeleteQuestion")]
        public async Task<ActionResult<object>> DeleteAsync(
            [FromRoute]
            Guid id, CancellationToken cancellationToken)
        {
            await _Sender.Send(new Delete.Command(id), cancellationToken);

            return Ok();
        }
    }
}
