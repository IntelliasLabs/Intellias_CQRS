using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// ExecutionError
    /// </summary>
    public class ExecutionError
    {
        /// <summary>
        /// Successful Execution Result
        /// </summary>
        [JsonConstructor]
        protected ExecutionError()
        {
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="message">Reason of failure</param>
        public ExecutionError(string message)
        {
            Message = message;
            Code = ErrorCodes.UnhandledError;
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message">Error Message</param>
        public ExecutionError(string source, string message)
        {
            Source = source;
            Message = message;
            Code = ErrorCodes.ValidationFailed;
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="code"></param>
        /// <param name="source"></param>
        /// <param name="message">Error Message</param>
        public ExecutionError(string code, string source, string message)
        {
            Code = code;
            Source = source;
            Message = message;
        }

        /// <summary>
        /// Error code
        /// </summary>
        [JsonProperty]
        public string Code { get; protected set; } = string.Empty;

        /// <summary>
        /// Error Source, optional
        /// </summary>
        [JsonProperty]
        public string Source { get; protected set; } = string.Empty;

        /// <summary>
        /// Reason of failure
        /// </summary>
        [JsonProperty]
        public string Message { get; protected set; } = string.Empty;

        /// <summary>
        /// ErrorMessage
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Code}: '{Message}'";
        }
    }
}
