using System.Text.RegularExpressions;
using FluentValidation;
using Users.Domain.Entities.Users;

namespace Users.Domain.Validations
{
    internal class ApplicationUserValidator : AbstractValidator<ApplicationUser>
    {
        public readonly Regex _PhoneNumberValidator = new("^([+]?[\\s0-9]+)?(\\d{3}|[(]?[0-9]+[)])?([-]?[\\s]?[0-9])+$", RegexOptions.Compiled);

        public ApplicationUserValidator()
        {
            RuleFor(e => e.Id)
                .NotEmpty().WithMessage("Debes incluir el identificador del usuario");

            RuleFor(e => e.UserName)
                .NotEmpty().WithMessage("Debes indicar tu nombre de usuario");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Debes indicar un correo")
                .EmailAddress().WithMessage("El correo ingresado ({PropertyValue}) no tiene el formato válido (Formato ab@cd.ef)");

            RuleFor(e => e.PhoneNumber)
                .NotEmpty().WithMessage("Debes indicar tu número de teléfono")
                .Matches(_PhoneNumberValidator).WithMessage("El número ingresado ({PropertyValue}) no tiene el formato válido (Formato +569 4321 1234)");

            RuleForEach(e => e.LoginInformations)
                .ChildRules(validator =>
                {
                    validator.RuleFor(e => e.Id)
                        .NotEmpty().WithMessage("Debes incluir el identificador de registro de inicio de sesión");

                    validator.RuleFor(e => e.Ip)
                        .MaximumLength(UserLoginInformation.IpMaxLength)
                            .WithMessage("La IP enviada ({PropertyName}) tiene {MaxLength} caracteres, el máximo es: {TotalLength}");

                    validator.RuleFor(e => e.Continent)
                        .MaximumLength(UserLoginInformation.ContinentMaxLength)
                            .WithMessage("El continente enviado ({PropertyName}) tiene {MaxLength} caracteres, el máximo es: {TotalLength}");

                    validator.RuleFor(e => e.Region)
                        .MaximumLength(UserLoginInformation.RegionMaxLength)
                            .WithMessage("La región enviada ({PropertyName}) tiene {MaxLength} caracteres, el máximo es: {TotalLength}");

                    validator.RuleFor(e => e.City)
                        .MaximumLength(UserLoginInformation.CityMaxLength)
                            .WithMessage("La ciudad enviada ({PropertyName}) tiene {MaxLength} caracteres, el máximo es: {TotalLength}");

                });
        }
    }
}
