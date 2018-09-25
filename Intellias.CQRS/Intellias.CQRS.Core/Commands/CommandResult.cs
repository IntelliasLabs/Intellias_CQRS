namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc />
    public class CommandResult : ICommandResult
    {
        private CommandResult() { }

        private CommandResult(string failureReason)
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
        public static CommandResult Success { get; } = new CommandResult();

        /// <summary>
        /// Fail result
        /// </summary>
        /// <param name="reason">Reason of failure</param>
        /// <returns></returns>
        public static CommandResult Fail(string reason)
        {
            return new CommandResult(reason);
        }
    }
}
