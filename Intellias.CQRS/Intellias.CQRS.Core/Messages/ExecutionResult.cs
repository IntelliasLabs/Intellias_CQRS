namespace Intellias.CQRS.Core.Messages
{
    /// <inheritdoc />
    public class ExecutionResult : IExecutionResult
    {
        /// <summary>
        /// Execution Result
        /// </summary>
        protected ExecutionResult() { }

        /// <summary>
        /// Execution Result
        /// </summary>
        /// <param name="failureReason">Reason of failure</param>
        protected ExecutionResult(string failureReason)
        {
            FailureReason = failureReason;
        }

        /// <summary>
        /// Reason of failure
        /// </summary>
        public string FailureReason { get; }

        /// <summary>
        /// Is result successful
        /// </summary>
        public bool IsSuccess => string.IsNullOrEmpty(FailureReason);

        /// <summary>
        /// Succesful result
        /// </summary>
        public static ExecutionResult Success { get; } = new ExecutionResult();

        /// <summary>
        /// Fail result
        /// </summary>
        /// <param name="reason">Reason of failure</param>
        /// <returns></returns>
        public static ExecutionResult Fail(string reason)
        {
            return new ExecutionResult(reason);
        }
    }
}
