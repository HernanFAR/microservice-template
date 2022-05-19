using MediatR;
using Microsoft.EntityFrameworkCore;
using Questions.EntityFramework;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Application.Features
{
    public class ReadOne
    {
        [DisplayName("ReadOneExampleDTO")]
        public record DTO(Guid Id, string Name);

        [DisplayName("ReadOneExampleQuery")]
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
                return await _Context.Questions
                    .Where(e => e.Id == request.Id)
                    .Select(e => new DTO(e.Id, e.Name))
                    .SingleOrDefaultAsync(cancellationToken);
            }
        }
    }
}
