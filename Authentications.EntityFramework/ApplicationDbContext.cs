﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Authentications.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentications.EntityFramework
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public async Task UseTransaction(Func<Task> action, CancellationToken cancellationToken)
        {
            var transaction = await Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await action();

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        public async Task<T> UseTransaction<T>(Func<Task<T>> action, CancellationToken cancellationToken)
        {
            var transaction = await Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await action();

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
