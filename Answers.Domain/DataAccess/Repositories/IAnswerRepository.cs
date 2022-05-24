using Answers.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Answers.Domain.DataAccess.Repositories
{
    public interface IAnswerRepository
    {
        Task<Answer> GetAsync(Guid id, CancellationToken cancellationToken);

        Task<Answer> CreateAsync(Answer entity, CancellationToken cancellationToken = default);

        Task<Answer> UpdateAsync(Answer entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
