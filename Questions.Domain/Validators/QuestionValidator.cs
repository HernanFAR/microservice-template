using FluentValidation;
using Questions.Domain.Entities;

namespace Questions.Domain.Validators
{
    public class QuestionValidator : AbstractValidator<Question>
    {
        public QuestionValidator()
        {
            RuleFor(e => e.Id)
                .NotEmpty().WithMessage("Debes especificar el identificador de la pregunta");

            RuleFor(e => e.Name)
                .NotEmpty()
                    .WithMessage("El nombre de la pregunta no puede ser nulo o estar vacío")
                .MaximumLength(Question.NameMaxLength)
                    .WithMessage("El nombre de la pregunta no puede tener más de {MaxLength} caracteres (Ingresaste {TotalLength}).");

            RuleFor(e => e.CreatedById)
                .NotEmpty().WithMessage("Debes especificar quien creo la pregunta");

            RuleFor(e => e.Updated)
                .GreaterThan(e => e.Created)
                    .WithMessage("La fecha de actualización debe ser mayor a la de creación");

        }
    }
}
