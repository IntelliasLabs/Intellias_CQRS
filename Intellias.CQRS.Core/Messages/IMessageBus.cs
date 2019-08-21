using System.Threading.Tasks;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Abstraction of Message Bus.
    /// </summary>
    /// <typeparam name="T">Message Type.</typeparam>
    public interface IMessageBus<in T>
        where T : IMessage
    {
        /// <summary>
        /// Publishing an message.
        /// </summary>
        /// <param name="msg">Event instance.</param>
        /// <returns>Task.</returns>
        Task<IExecutionResult> PublishAsync(T msg);
    }
}
