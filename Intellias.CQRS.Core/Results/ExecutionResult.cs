using System;
using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results
{
    /// <inheritdoc />
    public sealed class ExecutionResult : IExecutionResult
    {
        /// <summary>
        /// Successful Execution Result
        /// </summary>
        [JsonConstructor]
        private ExecutionResult()
        {
        }

        /// <summary>
        /// Failed Execution Result
        /// </summary>
        private ExecutionResult(ExecutionError error)
        {
            Success = false;
            Error = error;
        }

        /// <summary>
        /// Error of execution
        /// </summary>
        [JsonProperty]
        public ExecutionError? Error { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        public bool Success { get; private set; } = true;

        /// <summary>
        /// Succesful result
        /// </summary>
        public static ExecutionResult Successful { get; } = new ExecutionResult();

        /// <summary>
        /// Failed result
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static ExecutionResult Failed(ExecutionError error) => new ExecutionResult(error);

        /// <summary>
        /// Failed result
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ExecutionResult Failed(string error, Exception? ex = null) => Failed(new ExecutionError(error, ex));
    }
}
