using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Abstraction of Event Bus
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishing an event
        /// </summary>
        /// <typeparam name="T">Type of event</typeparam>
        /// <param name="event">Event instance</param>
        /// <returns>Task</returns>
        Task PublishAsync<T>(T @event) where T : IEvent;
    }
}
