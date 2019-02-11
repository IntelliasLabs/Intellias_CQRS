using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
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
        Task<IExecutionResult> HandleAsync(T @event);
    }
}
