using SharedKernel.Domain.Interfaces;
using SharedKernel.Domain.Others;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharedKernel.Domain.Abstracts
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        private readonly ICollection<EventInformation> _Events = new Collection<EventInformation>();

        public IEnumerable<EventInformation> GetEvents() => _Events;

        public void AddEvent(EventInformation eventData) => _Events.Add(eventData);

        public void ClearEvents() => _Events.Clear();

        public abstract void Validate();
    }

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
    {
        private readonly ICollection<EventInformation> _Events = new Collection<EventInformation>();

        protected AggregateRoot() { }

        protected AggregateRoot(TKey id) : base(id) { }

        public IEnumerable<EventInformation> GetEvents() => _Events;

        public void AddEvent(EventInformation eventData) => _Events.Add(eventData);

        public void ClearEvents() => _Events.Clear();

        public abstract void Validate();
    }
}
