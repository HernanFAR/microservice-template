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
        [DisplayName("UpdateQuestionDTO")]
        public record DTO(Guid Id, string Name, DateTime Created, DateTime Updated);

        [DisplayName("UpdateQuestionCommand")]
        public record Command(Guid Id, string Name) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly IQuestionUnitOfWork _UnitOfWork;
            private readonly ApplicationDbContext _Context;
            private readonly ITimeProvider _TimeProvider;
            private readonly HttpContext _HttpContext;

            public Handler(IQuestionUnitOfWork unitOfWork, ApplicationDbContext context,
                ITimeProvider timeProvider, IHttpContextAccessor contextAccessor)
            {
                _UnitOfWork = unitOfWork;
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
                    throw BusinessException.NotFound();
                }

                var originalExample = await _UnitOfWork.QuestionRepository
                    .GetAsync(id, cancellationToken);

                originalExample.UpdateState(name, updatedById, _TimeProvider.GetDateTime());

                await _UnitOfWork.UseTransactionAsync(async () =>
                {
                    await _UnitOfWork.QuestionRepository.UpdateAsync(originalExample, cancellationToken);

                    await _UnitOfWork.SaveChangesAsync(cancellationToken);

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
                        .WithMessage("La pregunta no puede ser nulo o estar vacío")
                    .MaximumLength(Question.NameMaxLength)
                        .WithMessage("La pregunta no puede tener más de {MaxLength} caracteres (Ingresaste {TotalLength}).");
            }
        }
    }
}
