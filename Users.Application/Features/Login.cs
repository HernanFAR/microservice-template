using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using SharedKernel.Domain.Others;
using Users.Application.Abstractions;
using Users.EntityFramework;
using Users.EntityFramework.Identity;

namespace Users.Application.Features
{
    public class Login
    {
        [DisplayName("AuthenticationLoginDTO")]
        public record DTO(string Token, DateTimeOffset ExpireDate);

        [DisplayName("AuthenticationLoginCommand")]
        public record Command(string Email, string Password) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly ApplicationDbContext _Context;
            private readonly ApplicationSignInManager _SignInManager;
            private readonly ITokenGenerator _TokenGenerator;
            private readonly ITimeProvider _TimeProvider;
            private readonly IDoxer _Doxer;
            private readonly HttpContext _HttpContext;

            public Handler(ApplicationDbContext context, ApplicationSignInManager signInManager, ITokenGenerator tokenGenerator,
                IHttpContextAccessor accessor, ITimeProvider timeProvider, IDoxer doxer)
            {
                _Context = context;
                _SignInManager = signInManager;
                _TokenGenerator = tokenGenerator;
                _TimeProvider = timeProvider;
                _Doxer = doxer;
                _HttpContext = accessor.HttpContext!;
            }

            public async Task<DTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _SignInManager.UserManager.FindByEmailAsync(request.Email);

                if (user is null)
                {
                    throw BusinessException.NotFound($"No existe un usuario con correo {request.Email} en el sistema");
                }

                var result = await _SignInManager.PasswordSignInAsync(user, request.Password, false, true);

                if (result.IsLockedOut)
                {
                    throw BusinessException.Forbidden($"Estas bloqueado por {user.LockoutEnd} más");
                }

                if (result.IsNotAllowed)
                {
                    throw BusinessException.Forbidden("No tienes permitido el inicio de sesión");
                }

                if (!result.Succeeded)
                {
                    throw BusinessException.Unauthorized("La contraseña no es correcta");
                }
                
                var ip = _HttpContext.Connection.RemoteIpAddress;

                if (ip is not null)
                {
                    var ipString = ip.MapToIPv4().ToString();
                    var doxInfo = await _Doxer.DoxIpAsync(ipString, cancellationToken);

                    await _Context.UseTransaction(async () =>
                    {
                        var loginInformation = user.AddLoginInformation(ipString,
                            doxInfo?.Continent, doxInfo?.Region, doxInfo?.City,
                            doxInfo?.Latitude, doxInfo?.Longitude, _TimeProvider.GetDateTimeOffset());

                        await _Context.AddAsync(loginInformation, cancellationToken);

                        await _Context.SaveChangesAsync(cancellationToken);

                    }, cancellationToken);
                }

                var (token, validTo) = await _TokenGenerator.GetIdentityTokenAsync(user, cancellationToken);

                return new DTO(token, validTo);
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(e => e.Email)
                    .NotEmpty().WithMessage("Debes indicar un correo")
                    .EmailAddress().WithMessage("El correo ingresado ({PropertyValue}) no tiene el formato válido (Formato ab@cd.ef)");

                RuleFor(e => e.Password)
                    .NotEmpty().WithMessage("Debes ingresar la contraseña");

            }
        }
    }
}
