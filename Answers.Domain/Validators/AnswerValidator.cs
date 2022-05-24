using Answers.Domain.Entities;
using FluentValidation;

namespace Answers.Domain.Validators
{
    public class AnswerValidator : AbstractValidator<Answer>
    {
        public AnswerValidator()
        {
            RuleFor(e => e.Id)
                .NotEmpty().WithMessage("Debes especificar el identificador de la respuesta");

            RuleFor(e => e.Name)
                .NotEmpty()
                    .WithMessage("El nombre de la respuesta no puede ser nulo o estar vacío")
                .MaximumLength(Answer.NameMaxLength)
                    .WithMessage("El nombre de la respuesta no puede tener más de {MaxLength} caracteres (Ingresaste {TotalLength}).");

            RuleFor(e => e.CreatedById)
                .NotEmpty().WithMessage("Debes especificar quien creo la respuesta");

            RuleFor(e => e.Updated)
                .GreaterThan(e => e.Created)
                    .WithMessage("La fecha de actualización debe ser mayor a la de creación");

        }
    }
}
