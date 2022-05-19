using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Questions.Domain.DataAccess;
using Questions.Domain.Entities;
using Questions.EntityFramework;
using SharedKernel.Domain.Others;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Application.Features
{
    public class Update
    {
        [DisplayName("UpdateQuestionCommand")]
        public record Command(Guid Id, string Name, Guid UpdatedById) : IRequest
        {
            public Guid UpdatedById { get; set; } = UpdatedById;
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IQuestionUnitOfWork _QuestionUnitOfWork;
            private readonly ApplicationDbContext _Context;
            private readonly ITimeProvider _TimeProvider;

            public Handler(IQuestionUnitOfWork questionUnitOfWork, ApplicationDbContext context,
                ITimeProvider timeProvider)
            {
                _QuestionUnitOfWork = questionUnitOfWork;
                _Context = context;
                _TimeProvider = timeProvider;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var (id, name, updatedById) = request;
                var existExample = await _Context.Questions
                    .Where(e => e.CreatedById == request.UpdatedById)
                    .AnyAsync(e => e.Id == id, cancellationToken);

                if (!existExample)
                {
                    throw BusinessException.NotFoundWithMessage("No se ha encontrado un ejemplo con ese identificador");
                }

                var originalExample = await _QuestionUnitOfWork.QuestionRepository
                    .GetAsync(id, cancellationToken);

                originalExample.UpdateState(name, updatedById, _TimeProvider.GetDateTime());

                originalExample.Validate();

                await _QuestionUnitOfWork.UseTransactionAsync(async () =>
                {
                    await _QuestionUnitOfWork.QuestionRepository.UpdateAsync(originalExample, cancellationToken);

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
