using Authentications.Application.Features;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.WebAPI.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Authentications.WebAPI.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [SwaggerTag("Contiene funciones relacionadas a la gestión del perfil")]
    public class ProfileController : ControllerBase
    {
        private readonly ISender _Sender;

        public ProfileController(ISender sender)
        {
            _Sender = sender;
        }

        [SwaggerOperation(Summary = "Obtiene la información de perfil del usuario en sesión")]

        [SwaggerResponse(StatusCodes.Status200OK,
            "Se ha obtenido correctamente la información del perfil en sesión",
            typeof(ControllerResponse<Profile.DTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No existe una cuenta en el sistema el identificador del Token ingresado",
            typeof(ControllerResponse<object>))]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [HttpGet]
        public async Task<ActionResult<ControllerResponse<Profile.DTO>>> Get(CancellationToken cancellationToken)
        {
            var dto = await _Sender.Send(new Profile.Query(), cancellationToken);

            return Ok(ControllerResponse<Profile.DTO>.SuccessWith(dto));
        }
    }
}
