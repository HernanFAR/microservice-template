using SharedKernel.Domain.Others;
using System.Collections.Generic;

namespace SharedKernel.Domain.Interfaces
{
    public interface IAggregateRoot : IEntity
    {
        IEnumerable<EventInformation> GetEvents();

        void AddEvent(EventInformation eventData);

        void ClearEvents();

        void Validate();

    }

    public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey> { }
}
