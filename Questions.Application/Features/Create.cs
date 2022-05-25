using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        [DisplayName("CreateQuestionDTO")]
        public record DTO(Guid Id, string Name, DateTime Created);

        [DisplayName("CreateQuestionCommand")]
        public record Command(string Name) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly IQuestionUnitOfWork _UnitOfWork;
            private readonly ITimeProvider _TimeProvider;
            private readonly HttpContext _Context;

            public Handler(IQuestionUnitOfWork unitOfWork, ITimeProvider timeProvider,
                IHttpContextAccessor contextAccessor)
            {
                _UnitOfWork = unitOfWork;
                _TimeProvider = timeProvider;
                _Context = contextAccessor.HttpContext!;
            }

            public async Task<DTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var createdById = _Context.GetIdentityId();

                var question = new Question(request.Name, createdById, _TimeProvider.GetDateTime());

                await _UnitOfWork.UseTransactionAsync(async () =>
                {
                    await _UnitOfWork.QuestionRepository.CreateAsync(question, cancellationToken);

                    await _UnitOfWork.SaveChangesAsync(cancellationToken);

                }, cancellationToken);

                return new DTO(question.Id, question.Name, question.Created);
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
