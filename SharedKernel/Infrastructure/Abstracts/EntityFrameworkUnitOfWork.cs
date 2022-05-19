using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Abstracts
{
    public abstract class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _Context;

        protected IDbContextTransaction? DbContextTransaction;

        protected EntityFrameworkUnitOfWork(DbContext context)
        {
            _Context = context;
        }

        public virtual async Task CreateTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (DbContextTransaction != null)
            {
                return;
            }

            DbContextTransaction = await _Context.Database.BeginTransactionAsync(cancellationToken);
        }

        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (DbContextTransaction is null)
            {
                return;
            }

            await DbContextTransaction.CommitAsync(cancellationToken);
        }

        public virtual async Task FinishTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (DbContextTransaction is null)
            {
                return;
            }

            await DbContextTransaction.DisposeAsync();

            DbContextTransaction = null;
        }

        public virtual Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (DbContextTransaction is null)
            {
                return Task.CompletedTask;
            }

            return DbContextTransaction.RollbackAsync(cancellationToken);
        }

        public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UseTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            await CreateTransactionAsync(cancellationToken);

            try
            {
                await action();

                await CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await RollbackAsync(cancellationToken);

                throw;
            }
            finally
            {
                await FinishTransactionAsync(cancellationToken);
            }
        }

        public virtual async Task<T> UseTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            await CreateTransactionAsync(cancellationToken);

            try
            {
                var toReturn = await action();

                await CommitAsync(cancellationToken);

                return toReturn;
            }
            catch (Exception)
            {
                await RollbackAsync(cancellationToken);

                throw;
            }
            finally
            {
                await FinishTransactionAsync(cancellationToken);
            }
        }
    }
}
