using System.Threading.Tasks;

namespace Product.Domain.Core.Events
{
    /// <summary>
    /// Abstraction of Event Bus
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Bublish an event
        /// </summary>
        /// <typeparam name="T">Type of event</typeparam>
        /// <param name="event">Event instance</param>
        /// <returns>Task</returns>
        Task Publish<T>(T @event) where T : IEvent;
    }
}
