using FluentValidation;
using MediatR;
using Questions.Domain.DataAccess;
using Questions.Domain.Entities;
using SharedKernel.Domain.Others;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Application.Features
{
    public class Create
    {
        [DisplayName("CreateQuestionCommand")]
        public record Command(string Name, Guid CreatedById) : IRequest
        {
            public Guid CreatedById { get; set; } = CreatedById;
        };

        public class Handler : IRequestHandler<Command>
        {
            private readonly IQuestionUnitOfWork _QuestionUnitOfWork;
            private readonly ITimeProvider _TimeProvider;

            public Handler(IQuestionUnitOfWork questionUnitOfWork, ITimeProvider timeProvider)
            {
                _QuestionUnitOfWork = questionUnitOfWork;
                _TimeProvider = timeProvider;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var (name, createdById) = request;

                var example = new Question(name, createdById, _TimeProvider.GetDateTime());

                example.Validate();

                await _QuestionUnitOfWork.UseTransactionAsync(async () =>
                {
                    await _QuestionUnitOfWork.QuestionRepository.CreateAsync(example, cancellationToken);

                    await _QuestionUnitOfWork.SaveChangesAsync(cancellationToken);

                }, cancellationToken);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(e => e.Name)
                    .NotEmpty()
                        .WithMessage("El nombre de la pregunta no puede ser nulo o estar vacío")
                    .MaximumLength(Question.NameMaxLength)
                        .WithMessage("El nombre de la pregunta no puede tener más de {MaxLength} caracteres (Ingresaste {TotalLength}).");
            }
        }
    }
}
