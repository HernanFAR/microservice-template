using Microsoft.EntityFrameworkCore;
using Questions.Domain.DataAccess.Repositories;
using Questions.Domain.Entities;
using Questions.EntityFramework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Infrastructure.DataAccess
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _Context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<Question> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _Context.Questions
                .Where(e => e.Id == id)
                .SingleAsync(cancellationToken);
        }

        public async Task<Question> CreateAsync(Question entity,
            CancellationToken cancellationToken = default)
        {
            var example = await _Context.Questions
                .AddAsync(entity, cancellationToken);

            return example.Entity;
        }

        public Task<Question> UpdateAsync(Question entity,
            CancellationToken cancellationToken = default)
        {
            var example = _Context.Questions
                .Update(entity);

            return Task.FromResult(example.Entity);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var example = await _Context.Questions
                .FindAsync(new object[] { id }, cancellationToken);

            _Context.Questions.Remove(example);

        }
    }
}
