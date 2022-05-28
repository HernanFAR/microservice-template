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
    public class ReadAll
    {
        [DisplayName("ReadAllAnswerDTO")]
        public record DTO(Guid Id, string Name, Guid QuestionId, DateTime Created);

        [DisplayName("ReadAllAnswerDTO")]
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
                return await _Context.Answers
                    .Select(e => new DTO(e.Id, e.Name, e.QuestionId, e.Created))
                    .ToListAsync(cancellationToken);
            }
        }

    }
}
