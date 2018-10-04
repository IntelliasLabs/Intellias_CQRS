using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc cref="IEventResult" />
    public class EventResult : ExecutionResult, IEventResult
    {
        /// <summary>
        /// Execution Result
        /// </summary>
        protected EventResult() { }

        /// <summary>
        /// Execution Result
        /// </summary>
        /// <param name="failureReason">Reason of failure</param>
        protected EventResult(string failureReason) : base(failureReason)
        {
        }

        /// <summary>
        /// Succesful result
        /// </summary>
        public static EventResult Success { get; } = new EventResult();

        /// <summary>
        /// Fail result
        /// </summary>
        /// <param name="reason">Reason of failure</param>
        /// <returns></returns>
        public static EventResult Fail(string reason)
        {
            return new EventResult(reason);
        }
    }
}
