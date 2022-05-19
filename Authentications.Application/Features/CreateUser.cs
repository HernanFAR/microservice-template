using Authentications.Infrastructure;
using MediatR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Application.Abstractions;
using Authentications.Application.Configurations;
using Authentications.Application.Extensions;
using Authentications.Domain.Entities;
using Authentications.EntityFramework;
using Authentications.EntityFramework.Identity;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Domain.Others;

namespace Authentications.Application.Features
{
    public class CreateUser
    {
        public record DTO(string Token, DateTimeOffset ExpireDate);

        public record Command(string Name, string Email, string PhoneNumber, string Password, string ConfirmPassword) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly ApplicationUserManager _UserManager;
            private readonly ApplicationDbContext _Context;

            private readonly ITokenGenerator _TokenGenerator;
            private readonly ITimeProvider _TimeProvider;

            public Handler(ApplicationUserManager userManager, ApplicationDbContext context, 
                ITokenGenerator tokenGenerator,
                ITimeProvider timeProvider)
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

                    var addUserResult = await _UserManager
                        .CreateAsync(user, password);

                    addUserResult.ThrowValidationExceptionIfNotValid();

                    return user;

                }, cancellationToken);

                var (token, validTo) = await _TokenGenerator.GetIdentityTokenAsync(user, cancellationToken);

                return new DTO(token, validTo);
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly ApplicationDbContext _Context;

            public Validator(ApplicationDbContext context)
            {
                _Context = context;

                RuleFor(e => e.Name)
                    .NotEmpty().WithMessage("Debes indicar tu nombre");

                RuleFor(e => e.Email)
                    .NotEmpty().WithMessage("Debes indicar un correo")
                    .EmailAddress().WithMessage("El correo debe ser valido")
                    .MustAsync(BeANonExistingEmail).WithMessage("El correo ya existe en el sistema");

                RuleFor(e => e.PhoneNumber)
                    .NotEmpty().WithMessage("Debes indicar tu número de teléfono");

            }

            private async Task<bool> BeANonExistingEmail(string email, CancellationToken cancellationToken)
            {
                return await _Context.Users
                    .AnyAsync(e => e.Email == email, cancellationToken);
            }
        }
    }
}
