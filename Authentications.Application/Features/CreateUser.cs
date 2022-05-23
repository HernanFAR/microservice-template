using Authentications.Application.Abstractions;
using Authentications.Domain.Entities.Users;
using Authentications.EntityFramework;
using Authentications.EntityFramework.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Others;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Authentications.Application.Features
{
    public class CreateUser
    {
        [DisplayName("CreateUserLoginDTO")]
        public record DTO(string Token, DateTimeOffset ExpireDate);

        [DisplayName("CreateUserLoginCommand")]
        public record Command(string UserName, string Email, string PhoneNumber, string Password, string ConfirmPassword) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly ApplicationUserManager _UserManager;
            private readonly ApplicationDbContext _Context;

            private readonly ITokenGenerator _TokenGenerator;
            private readonly ITimeProvider _TimeProvider;

            public Handler(ApplicationUserManager userManager, ApplicationDbContext context,
                ITokenGenerator tokenGenerator, ITimeProvider timeProvider)
            {
                _UserManager = userManager;
                _Context = context;
                _TokenGenerator = tokenGenerator;
                _TimeProvider = timeProvider;
            }

            public async Task<DTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _Context.UseTransaction(async () =>
                {
                    var (name, email, phoneNumber, password, _) = request;

                    var user = new ApplicationUser(_TimeProvider.GetDateTime())
                    {
                        UserName = name,
                        Email = email,
                        PhoneNumber = phoneNumber
                    };

                    var result = await _UserManager.CreateAsync(user, password);

                    result.ThrowValidationExceptionIfNotValid();

                    return user;

                }, cancellationToken);

                var (token, validTo) = await _TokenGenerator.GetIdentityTokenAsync(user, cancellationToken);

                return new DTO(token, validTo);
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _Context;
            private readonly Regex _PhoneNumberValidator = new("^([+]?[\\s0-9]+)?(\\d{3}|[(]?[0-9]+[)])?([-]?[\\s]?[0-9])+$", RegexOptions.Compiled);

            public Validator(ApplicationDbContext context)
            {
                _Context = context;

                RuleFor(e => e.UserName)
                    .NotEmpty().WithMessage("Debes indicar tu nombre de usuario");

                RuleFor(e => e.Email)
                    .NotEmpty().WithMessage("Debes indicar un correo")
                    .EmailAddress().WithMessage("El correo ingresado ({PropertyValue}) no tiene el formato válido (Formato ab@cd.ef)")
                    .MustAsync(BeANonExistingEmail).WithMessage("El correo ingresado ({PropertyValue}) ya existe en el sistema");

                RuleFor(e => e.PhoneNumber)
                    .NotEmpty().WithMessage("Debes indicar tu número de teléfono")
                    .Matches(_PhoneNumberValidator).WithMessage("El número ingresado ({PropertyValue}) no tiene el formato válido (Formato +569 4321 1234)");

                RuleFor(e => e.ConfirmPassword)
                    .Equal(e => e.ConfirmPassword)
                    .WithMessage("La contraseña y la confirmación de contraseña no son iguales");

            }

            private async Task<bool> BeANonExistingEmail(string email, CancellationToken cancellationToken)
            {
                var exist = await _Context.Users
                    .AnyAsync(e => e.Email == email, cancellationToken);

                return !exist;
            }
        }
    }
}
