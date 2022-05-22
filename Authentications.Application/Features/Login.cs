using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;
using Authentications.Application.Configurations;
using Authentications.EntityFramework;
using Authentications.EntityFramework.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using SharedKernel.Application.Abstractions;
using SharedKernel.Domain.Others;

namespace Authentications.Application.Features
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
            private readonly IBackgroundTaskQueue _BackgroundTaskQueue;
            private readonly HttpContext _HttpContext;

            public Handler(ApplicationDbContext context, ApplicationSignInManager signInManager, ITokenGenerator tokenGenerator,
                IHttpContextAccessor accessor, ITimeProvider timeProvider, IDoxer doxer, IBackgroundTaskQueue backgroundTaskQueue)
            {
                _Context = context;
                _SignInManager = signInManager;
                _TokenGenerator = tokenGenerator;
                _TimeProvider = timeProvider;
                _Doxer = doxer;
                _BackgroundTaskQueue = backgroundTaskQueue;
                _HttpContext = accessor.HttpContext!;
            }

            public async Task<DTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _SignInManager.UserManager.FindByEmailAsync(request.Email);

                if (user is null)
                {
                    throw BusinessException.NotFoundWithMessage($"No existe un usuario con correo {request.Email} en el sistema");
                }

                var result = await _SignInManager.PasswordSignInAsync(user, request.Password, false, true);

                if (result.IsLockedOut)
                {
                    throw BusinessException.ForbiddenWithMessage($"Estas bloqueado por {user.LockoutEnd} más");
                }

                if (result.IsNotAllowed)
                {
                    throw BusinessException.ForbiddenWithMessage("No tienes permitido el inicio de sesión");
                }

                if (!result.Succeeded)
                {
                    throw BusinessException.UnauthorizedWithMessage("La contraseña no es correcta");
                }

                await _BackgroundTaskQueue.QueueBackgroundWorkItemAsync(async queueToken =>
                {
                    var ip = _HttpContext.Connection.RemoteIpAddress;

                    if (ip is not null)
                    {
                        var ipString = ip.MapToIPv4().ToString();
                        var doxInfo = await _Doxer.DoxIpAsync(ipString, queueToken);

                        await _Context.UseTransaction(async () =>
                        {
                            var loginInformation = user.AddLoginInformation(ipString,
                                doxInfo?.Continent, doxInfo?.Region, doxInfo?.City,
                                doxInfo?.Latitude, doxInfo?.Longitude, _TimeProvider.GetDateTimeOffset());

                            await _Context.AddAsync(loginInformation, queueToken);

                            await _Context.SaveChangesAsync(queueToken);

                        }, queueToken);
                    }
                });

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
