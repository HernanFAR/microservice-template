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
            private readonly IQuestionUnitOfWork _QuestionUnitOfWork;
            private readonly ITimeProvider _TimeProvider;
            private readonly HttpContext _Context;

            public Handler(IQuestionUnitOfWork questionUnitOfWork, ITimeProvider timeProvider,
                IHttpContextAccessor contextAccessor)
            {
                _QuestionUnitOfWork = questionUnitOfWork;
                _TimeProvider = timeProvider;
                _Context = contextAccessor.HttpContext!;
            }

            public async Task<DTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var createdById = _Context.GetIdentityId();

                var example = new Question(request.Name, createdById, _TimeProvider.GetDateTime());

                example.Validate();

                await _QuestionUnitOfWork.UseTransactionAsync(async () =>
                {
                    await _QuestionUnitOfWork.QuestionRepository.CreateAsync(example, cancellationToken);

                    await _QuestionUnitOfWork.SaveChangesAsync(cancellationToken);

                }, cancellationToken);

                return new DTO(example.Id, example.Name, example.Created);
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
