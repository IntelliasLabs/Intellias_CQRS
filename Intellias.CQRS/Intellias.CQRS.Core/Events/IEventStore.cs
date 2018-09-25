using System.Collections.Generic;
using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Event storage abstraction
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Store events
        /// </summary>
        /// <param name="events">Events</param>
        /// <returns>Awaiter</returns>
        Task Save(IEnumerable<IEvent> events);

        /// <summary>
        /// Get events for specified version of AR
        /// </summary>
        /// <param name="aggregateId">ARID</param>
        /// <param name="version">ARV</param>
        /// <returns>Events</returns>
        Task<IEnumerable<IEvent>> Get(string aggregateId, int version);
    }
}
