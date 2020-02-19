using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Pipelines
{
    /// <summary>
    /// Dispatches serialized message to handlers.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Dispatches command to a handlers.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DispatchCommandAsync(ICommand command);

        /// <summary>
        /// Dispatches event to a handler.
        /// </summary>
        /// <param name="event">Event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DispatchEventAsync(IEvent @event);
    }
}