using MediatR;
using Microsoft.AspNetCore.Http;
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
        public record Command(Guid Id) : IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly IQuestionUnitOfWork _UnitOfWork;
            private readonly ApplicationDbContext _Context;
            private readonly HttpContext _HttpContext;

            public Handler(IQuestionUnitOfWork unitOfWork, ApplicationDbContext context,
                IHttpContextAccessor contextAccessor)
            {
                _UnitOfWork = unitOfWork;
                _Context = context;
                _HttpContext = contextAccessor.HttpContext!;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var deletedById = _HttpContext.GetIdentityId();

                var baseQuery = _Context.Questions
                    .AsQueryable();

                if (!_HttpContext.HasRole("Admin"))
                {
                    baseQuery = baseQuery.Where(e => e.CreatedById == deletedById);
                }

                var existExample = await baseQuery
                    .AnyAsync(e => e.Id == request.Id, cancellationToken);

                if (!existExample)
                {
                    throw BusinessException.NotFoundWithMessage("No se ha encontrado un ejemplo con ese identificador");
                }

                await _UnitOfWork.UseTransactionAsync(async () =>
                {
                    await _UnitOfWork.QuestionRepository.DeleteAsync(request.Id, cancellationToken);

                    await _UnitOfWork.SaveChangesAsync(cancellationToken);

                }, cancellationToken);

                return Unit.Value;
            }
        }

    }
}
