using Questions.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Questions.Domain.DataAccess.Repositories
{
    public interface IQuestionRepository
    {
        Task<Question> GetAsync(Guid id, CancellationToken cancellationToken);

        Task<Question> CreateAsync(Question entity, CancellationToken cancellationToken = default);

        Task<Question> UpdateAsync(Question entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
