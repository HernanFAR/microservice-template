using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Answers.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Answers.Application.Features
{
    public class ReadOne
    {
        [DisplayName("ReadOneAnswerDTO")]
        public record DTO(Guid Id, string Name, Guid QuestionId, DateTime Created, DateTime? Updated);


        [DisplayName("ReadOneAnswerQuery")]
        public record Query(Guid Id) : IRequest<DTO?>;

        public class Handler : IRequestHandler<Query, DTO?>
        {
            private readonly ApplicationDbContext _Context;

            public Handler(ApplicationDbContext context)
            {
                _Context = context;
            }

            public async Task<DTO?> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _Context.Answers
                    .Where(e => e.Id == request.Id)
                    .Select(e => new DTO(e.Id, e.Name, e.QuestionId, e.Created, e.Updated))
                    .SingleOrDefaultAsync(cancellationToken);
            }
        }

    }
}
