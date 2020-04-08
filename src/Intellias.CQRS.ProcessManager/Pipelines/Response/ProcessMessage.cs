using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.ProcessManager.Pipelines.Response
{
    /// <summary>
    /// Process message.
    /// </summary>
    public class ProcessMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessMessage"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="isPublished">Is published.</param>
        public ProcessMessage(IMessage message, bool isPublished = false)
        {
            Message = message;
            IsPublished = isPublished;
        }

        /// <summary>
        /// Message.
        /// </summary>
        public IMessage Message { get; }

        /// <summary>
        /// Is published.
        /// </summary>
        public bool IsPublished { get; }
    }
}
