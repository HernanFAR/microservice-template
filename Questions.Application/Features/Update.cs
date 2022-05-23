using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public record DTO(Guid Id, string Name, DateTime Created, DateTime Updated);

        [DisplayName("UpdateQuestionCommand")]
        public record Command(Guid Id, string Name) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly IQuestionUnitOfWork _QuestionUnitOfWork;
            private readonly ApplicationDbContext _Context;
            private readonly ITimeProvider _TimeProvider;
            private readonly HttpContext _HttpContext;

            public Handler(IQuestionUnitOfWork questionUnitOfWork, ApplicationDbContext context,
                ITimeProvider timeProvider, IHttpContextAccessor contextAccessor)
            {
                _QuestionUnitOfWork = questionUnitOfWork;
                _Context = context;
                _TimeProvider = timeProvider;
                _HttpContext = contextAccessor.HttpContext!;
            }

            public async Task<DTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var (id, name) = request;
                var updatedById = _HttpContext.GetIdentityId();

                var baseQuery = _Context.Questions
                    .AsQueryable();

                if (!_HttpContext.HasRole("Admin"))
                {
                    baseQuery = baseQuery.Where(e => e.CreatedById == updatedById);
                }

                var existExample = await baseQuery
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

                return new DTO(originalExample.Id, originalExample.Name,
                    originalExample.Created, originalExample.Updated.GetValueOrDefault());
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
