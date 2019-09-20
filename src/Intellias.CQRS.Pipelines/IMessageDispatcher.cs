using System.Threading.Tasks;

namespace Intellias.CQRS.Pipelines
{
    /// <summary>
    /// Dispatches serialized message to handlers.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Dispatches event to a handler.
        /// </summary>
        /// <param name="message">Serialized message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DispatchEventAsync(string message);
    }
}