using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Domain.Interfaces;
using SharedKernel.Domain.Others;

namespace SharedKernel.Infrastructure.EntityFrameworkCore.Interceptors
{
    public class EventInvokerInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _Publisher;

        public EventInvokerInterceptor(IPublisher publisher)
        {
            _Publisher = publisher;
        }

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            var aggregateRoots = eventData.Context
                .ChangeTracker.Entries()
                .Where(e => e.Entity != null && e is { Entity: IAggregateRoot _ })
                .Select(e => (IAggregateRoot)e.Entity)
                .Where(e => e.GetEvents().Any());

            await ProcessEvents(aggregateRoots, true, cancellationToken);

            return result;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var aggregateRoots = eventData.Context
                .ChangeTracker.Entries()
                .Where(e => e.Entity != null && e is { Entity: IAggregateRoot _ })
                .Select(e => (IAggregateRoot)e.Entity)
                .Where(e => e.GetEvents().Any());

            await ProcessEvents(aggregateRoots, false, cancellationToken);

            return result;
        }

        private async Task ProcessEvents(IEnumerable<IAggregateRoot> aggregateRoots, bool isAfterSafe, CancellationToken cancellationToken)
        {
            var notProcessedEvents = new List<EventInformation>();

            foreach (var aggregateRoot in aggregateRoots)
            {
                var events = aggregateRoot.GetEvents();

                foreach (var eventInformation in events)
                {
                    if (isAfterSafe != eventInformation.AfterSave)
                    {
                        notProcessedEvents.Add(eventInformation);

                        continue;
                    }

                    await _Publisher.Publish(eventInformation.EventData, cancellationToken);
                }

                aggregateRoot.ClearEvents();

                notProcessedEvents.ForEach(e => aggregateRoot.AddEvent(e));
                notProcessedEvents.Clear();
            }
        }
    }
}
