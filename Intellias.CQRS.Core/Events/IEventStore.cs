using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Event storage abstraction.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Store events.
        /// </summary>
        /// <param name="entity">IAggregateRoot.</param>
        /// <returns>Awaiter.</returns>
        Task<IEnumerable<IEvent>> SaveAsync(IAggregateRoot entity);

        /// <summary>
        /// Get events for specified version of AR.
        /// </summary>
        /// <param name="aggregateId">ARID.</param>
        /// <param name="fromVersion">ARV.</param>
        /// <returns>Events.</returns>
        Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion);
    }
}
