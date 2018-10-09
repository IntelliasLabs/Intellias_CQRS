using System.Collections.Generic;
using System.Collections.ObjectModel;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    /// <summary>
    /// AR abstraction
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Version of AR
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Holds the list of events
        /// </summary>
        ReadOnlyCollection<IEvent> Events { get; }

        /// <summary>
        /// Load event history
        /// </summary>
        /// <param name="history"></param>
        void LoadFromHistory(IEnumerable<IEvent> history);
    }
}
