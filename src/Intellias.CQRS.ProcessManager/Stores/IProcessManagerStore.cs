using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.ProcessManager.Pipelines.Response;

namespace Intellias.CQRS.ProcessManager.Stores
{
    /// <summary>
    /// Process store.
    /// </summary>
    public interface IProcessManagerStore
    {
        /// <summary>
        /// Persist commands.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <param name="messages">Messages.</param>
        /// <returns>Task.</returns>
        Task PersistMessagesAsync(string id, IReadOnlyCollection<IMessage> messages);

        /// <summary>
        /// Mark message as published.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <param name="message">Command.</param>
        /// <returns>Task.</returns>
        Task MarkMessageAsPublishedAsync(string id, IMessage message);

        /// <summary>
        /// Get process messages.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Process messages.</returns>
        Task<IReadOnlyCollection<ProcessMessage>> GetMessagesAsync(string id);
    }
}