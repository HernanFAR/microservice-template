using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.WebAPI.Filters;
using Swashbuckle.AspNetCore.Annotations;
using Users.Application.Features;

namespace Users.WebAPI.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [SwaggerTag("Contiene funciones relacionadas a la gestión del perfil")]
    [UseAPIKey]
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
            typeof(Profile.DTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound,
            "No existe una cuenta en el sistema el identificador del Token ingresado")]

        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]

        [HttpGet(Name = "GetProfile")]
        public async Task<ActionResult<Profile.DTO>> Profile(CancellationToken cancellationToken)
        {
            var dto = await _Sender.Send(new Profile.Query(), cancellationToken);

            if (dto is null) return NotFound();

            return Ok(dto);
        }
    }
}
