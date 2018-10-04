using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Abstraction of Message Bus
    /// </summary>
    public interface IMessageBus<in T, TR> where T : IMessage where TR : IExecutionResult
    {
        /// <summary>
        /// Publishing an message
        /// </summary>
        /// <param name="msg">Event instance</param>
        /// <returns>Task</returns>
        Task<TR> PublishAsync(T msg);
    }
}
