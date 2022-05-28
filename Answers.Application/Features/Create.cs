using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Answers.Application.InternalServices;
using Answers.Domain.DataAccess;
using Answers.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using SharedKernel.Domain.Others;

namespace Answers.Application.Features
{
    public class Create
    {
        [DisplayName("CreateExampleDTO")]
        public record DTO(Guid Id, string Name, Guid QuestionId, DateTime Created);

        [DisplayName("CreateExampleCommand")]
        public record Command(string Name, Guid QuestionId) : IRequest<DTO>;

        public class Handler : IRequestHandler<Command, DTO>
        {
            private readonly IAnswerUnitOfWork _UnitOfWork;
            private readonly ITimeProvider _TimeProvider;
            private readonly HttpContext _Context;

            public Handler(IAnswerUnitOfWork unitOfWork, ITimeProvider timeProvider, IHttpContextAccessor contextAccessor)
            {
                _UnitOfWork = unitOfWork;
                _TimeProvider = timeProvider;
                _Context = contextAccessor.HttpContext!;
            }

            public async Task<DTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var createdById = _Context.GetIdentityId();

                var (name, questionId) = request;
                var answer = new Answer(name, questionId, createdById, _TimeProvider.GetDateTime());

                await _UnitOfWork.UseTransactionAsync(async () =>
                {
                    await _UnitOfWork.AnswerRepository.CreateAsync(answer, cancellationToken);

                    await _UnitOfWork.SaveChangesAsync(cancellationToken);

                }, cancellationToken);

                return new DTO(answer.Id, answer.Name, answer.QuestionId, answer.Created);

            }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly IQuestionService _QuestionService;

            public Validator(IQuestionService questionService)
            {
                _QuestionService = questionService;

                RuleFor(e => e.Name)
                    .NotEmpty()
                        .WithMessage("El nombre de la pregunta no puede ser nulo o estar vacío")
                    .MaximumLength(Answer.NameMaxLength)
                        .WithMessage("El nombre de la pregunta no puede tener más de {MaxLength} caracteres (Ingresaste {TotalLength})."); ;

                RuleFor(e => e.QuestionId)
                    .MustAsync(BeAExistingQuestion).WithMessage("El identificador {PropertyValue} no tiene una pregunta relacionada");

            }

            private Task<bool> BeAExistingQuestion(Guid id, CancellationToken cancellationToken)
            {
                return _QuestionService.Exist(id, cancellationToken);
            }
        }
    }
}
