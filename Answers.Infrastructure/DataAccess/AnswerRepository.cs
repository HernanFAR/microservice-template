using Microsoft.EntityFrameworkCore;
using Answers.Domain.DataAccess.Repositories;
using Answers.Domain.Entities;
using Answers.EntityFramework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Answers.Infrastructure.DataAccess
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _Context;

        public AnswerRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<Answer> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _Context.Answers
                .Where(e => e.Id == id)
                .SingleAsync(cancellationToken);
        }

        public async Task<Answer> CreateAsync(Answer entity,
            CancellationToken cancellationToken = default)
        {
            var example = await _Context.Answers
                .AddAsync(entity, cancellationToken);

            return example.Entity;
        }

        public Task<Answer> UpdateAsync(Answer entity,
            CancellationToken cancellationToken = default)
        {
            var example = _Context.Answers
                .Update(entity);

            return Task.FromResult(example.Entity);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var example = await _Context.Answers
                .FindAsync(new object[] { id }, cancellationToken);

            _Context.Answers.Remove(example);

        }
    }
}
