using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Bus to report operation status to outside world (for example to presentation layer by SignalR or GraphQL Subscriptions).
    /// </summary>
    public interface IReportBus
    {
        /// <summary>
        /// Publishing an message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <typeparam name="TMessage">Message Type.</typeparam>
        /// <returns>Task.</returns>
        Task PublishAsync<TMessage>(TMessage message)
            where TMessage : IMessage;
    }
}
