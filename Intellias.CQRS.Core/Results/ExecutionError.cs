using System;
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
        /// <param name="errorMessage">Reason of failure</param>
        /// <param name="ex"></param>
        public ExecutionError(string errorMessage, Exception? ex = null)
        {
            ErrorMessage = errorMessage;
            Exception = ex;
            ErrorCode = ErrorCodes.UnhandledError;
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="source"></param>
        /// <param name="errorMessage">Error Message</param>
        public ExecutionError(string source, string errorMessage)
        {
            Source = source;
            ErrorMessage = errorMessage;
            ErrorCode = ErrorCodes.ValidationFailed;
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="source"></param>
        /// <param name="errorMessage">Error Message</param>
        /// <param name="ex"></param>
        public ExecutionError(string errorCode, string source, string errorMessage, Exception? ex = null)
        {
            ErrorCode = errorCode;
            Source = source;
            ErrorMessage = errorMessage;
            Exception = ex;
        }

        /// <summary>
        /// Exception, optional
        /// </summary>
        [JsonProperty]
        public Exception? Exception { get; protected set; }

        /// <summary>
        /// Error code
        /// </summary>
        [JsonProperty]
        public string ErrorCode { get; protected set; } = string.Empty;

        /// <summary>
        /// Error Source, optional
        /// </summary>
        [JsonProperty]
        public string Source { get; protected set; } = string.Empty;

        /// <summary>
        /// Reason of failure
        /// </summary>
        [JsonProperty]
        public string ErrorMessage { get; protected set; } = string.Empty;

        /// <summary>
        /// ErrorMessage
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ErrorMessage;
        }
    }
}
