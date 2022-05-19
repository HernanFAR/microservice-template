using MediatR;
using Microsoft.EntityFrameworkCore;
using Questions.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Application.Features
{
    public class ReadAll
    {
        [DisplayName("ReadAllExampleDTO")]
        public record DTO(Guid Id, string Name);

        [DisplayName("ReadAllExampleQuery")]
        public record Query() : IRequest<IReadOnlyList<DTO>>;

        public class Handler : IRequestHandler<Query, IReadOnlyList<DTO>>
        {
            private readonly ApplicationDbContext _Context;

            public Handler(ApplicationDbContext context)
            {
                _Context = context;
            }

            public async Task<IReadOnlyList<DTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _Context.Questions
                    .Select(e => new DTO(e.Id, e.Name))
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
