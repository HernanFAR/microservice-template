using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);

        Task CreateTransactionAsync(CancellationToken cancellationToken = default);

        Task FinishTransactionAsync(CancellationToken cancellationToken = default);

        Task UseTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);

        Task<T> UseTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
    }
}
