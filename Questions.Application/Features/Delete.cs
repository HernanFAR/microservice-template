using MediatR;
using Microsoft.EntityFrameworkCore;
using Questions.Domain.DataAccess;
using Questions.EntityFramework;
using SharedKernel.Domain.Others;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Application.Features
{
    public class Delete
    {
        [DisplayName("DeleteQuestionCommand")]
        public record Command(Guid Id, Guid CreatedById) : IRequest
        {
            public Guid CreatedById { get; set; } = CreatedById;
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IQuestionUnitOfWork _QuestionUnitOfWork;
            private readonly ApplicationDbContext _Context;

            public Handler(IQuestionUnitOfWork questionUnitOfWork, ApplicationDbContext context)
            {
                _QuestionUnitOfWork = questionUnitOfWork;
                _Context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var (id, createdById) = request;
                var existExample = await _Context.Questions
                    .Where(e => e.CreatedById == createdById)
                    .AnyAsync(e => e.Id == id, cancellationToken);

                if (!existExample)
                {
                    throw BusinessException.NotFoundWithMessage("No se ha encontrado un ejemplo con ese identificador");
                }


                await _QuestionUnitOfWork.UseTransactionAsync(async () =>
                {
                    await _QuestionUnitOfWork.QuestionRepository.DeleteAsync(id, cancellationToken);

                    await _QuestionUnitOfWork.SaveChangesAsync(cancellationToken);

                }, cancellationToken);

                return Unit.Value;
            }
        }

    }
}
