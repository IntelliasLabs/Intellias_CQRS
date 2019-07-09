using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// ExecutionError
    /// </summary>
    public sealed class ExecutionError
    {
        /// <summary>
        /// Successful Execution Result
        /// </summary>
        [JsonConstructor]
        private ExecutionError()
        {
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="errorMessage">Reason of failure</param>
        /// <param name="ex"></param>
        public ExecutionError(string errorMessage, Exception ex = null)
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
        public ExecutionError(string errorCode, string source, string errorMessage, Exception ex = null)
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
        public Exception Exception { get; private set; }

        /// <summary>
        /// Error code
        /// </summary>
        [JsonProperty]
        public string ErrorCode { get; private set; } = string.Empty;

        /// <summary>
        /// Error Source, optional
        /// </summary>
        [JsonProperty]
        public string Source { private set; get; } = string.Empty;

        /// <summary>
        /// Reason of failure
        /// </summary>
        [JsonProperty]
        public string ErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Execution Errors
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<ExecutionError> Errors => errors;

        /// <summary>
        /// Add Error
        /// </summary>
        /// <param name="error">Error</param>
        /// <inheritdoc />
        public void AddError(ExecutionError error)
        {
            errors.Add(error);
        }

        [JsonProperty]
        private List<ExecutionError> errors = new List<ExecutionError>();

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
