using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Defines a handler for a message.
    /// </summary>
    /// <typeparam name="T">Message type being handled</typeparam>
    /// <typeparam name="TR">Execution result type</typeparam>
    public interface IHandler<T, TR> where T : IMessage where TR : IExecutionResult
    {
        /// <summary>
        ///  Handles a message
        /// </summary>
        /// <param name="message">Message being handled</param>
        /// <returns>Task that represents handling of message</returns>
        Task<TR> Handle(T message);
    }
}
