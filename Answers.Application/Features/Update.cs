using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Answers.Domain.DataAccess;
using Answers.Domain.Entities;
using Answers.EntityFramework;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Others;

namespace Answers.Application.Features
{
    public class Update
    {
        [DisplayName("UpdateAnswerDTO")]
        public record DTO(Guid Id, string Name, Guid QuestionId, DateTime Created, DateTime Updated);

        [DisplayName("UpdateAnswerCommand")]
        public record Command(Guid Id, string Name) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly IAnswerUnitOfWork _UnitOfWork;
            private readonly ApplicationDbContext _Context;
            private readonly ITimeProvider _TimeProvider;
            private readonly HttpContext _HttpContext;

            public Handler(IAnswerUnitOfWork unitOfWork, ApplicationDbContext context,
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

                var baseQuery = _Context.Answers
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

                var originalExample = await _UnitOfWork.AnswerRepository
                    .GetAsync(id, cancellationToken);

                originalExample.UpdateState(name, updatedById, _TimeProvider.GetDateTime());

                await _UnitOfWork.UseTransactionAsync(async () =>
                {
                    await _UnitOfWork.AnswerRepository.UpdateAsync(originalExample, cancellationToken);

                    await _UnitOfWork.SaveChangesAsync(cancellationToken);

                }, cancellationToken);

                return new DTO(originalExample.Id, originalExample.Name, originalExample.QuestionId,
                    originalExample.Created, originalExample.Updated.GetValueOrDefault());
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(e => e.Name)
                    .NotEmpty()
                        .WithMessage("La respuesta no puede ser nula o estar vacía")
                    .MaximumLength(Answer.NameMaxLength)
                        .WithMessage("la respuesta no puede tener más de {MaxLength} caracteres (Ingresaste {TotalLength}).");
            }
        }

    }
}
