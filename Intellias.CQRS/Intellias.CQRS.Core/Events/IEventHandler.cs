using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc />
    /// <summary>
    /// Event handler abstraction
    /// </summary>
    /// <typeparam name="T">Type of event</typeparam>
    public interface IEventHandler<in T> where T : IEvent
    {
        /// <summary>
        ///  Handles an event
        /// </summary>
        /// <param name="event">Event being handled</param>
        /// <returns>Task that represents handling of message</returns>
        Task HandleAsync(T @event);
    }
}
