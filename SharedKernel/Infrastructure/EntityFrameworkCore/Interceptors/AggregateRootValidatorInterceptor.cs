using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Domain.Interfaces;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Interceptors
{
    public class AggregateRootValidatorInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var aggregateRoots = eventData.Context
                .ChangeTracker.Entries()
                .Where(e => e.Entity != null && e is { Entity: IAggregateRoot _ })
                .Select(e => e.Entity)
                .Cast<IAggregateRoot>();

            foreach (var aggregateRoot in aggregateRoots)
            {
                aggregateRoot.Validate();
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
