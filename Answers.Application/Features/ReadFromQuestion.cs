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
    public class ReadFromQuestion
    {
        [DisplayName("ReadFromQuestionAnswerDTO")]
        public record DTO(Guid Id, string Name, Guid QuestionId, Guid CreatedById, DateTime Created, Guid? UpdatedById, DateTime? Updated);

        [DisplayName("ReadFromQuestionAnswerDTO")]
        public record Query(Guid QuestionId) : IRequest<IReadOnlyList<DTO>>;

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
                    .Where(e => e.QuestionId == request.QuestionId)
                    .Select(e => new DTO(e.Id, e.Name, e.QuestionId, 
                        e.CreatedById, e.Created, e.UpdatedById, e.Updated))
                    .ToListAsync(cancellationToken);
            }
        }

    }
}
