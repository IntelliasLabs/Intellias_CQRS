using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// Failed Execution Result
    /// </summary>
    public sealed class FailedResult : ExecutionError, IExecutionResult
    {
        [JsonConstructor]
        private FailedResult()
        {
        }

        /// <summary>
        /// Failed Result
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        public FailedResult(string error, Exception? ex = null)
        {
            ErrorCode = ErrorCodes.UnhandledError;
            ErrorMessage = error;
            Exception = ex;
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="source"></param>
        /// <param name="errorMessage">Error Message</param>
        /// <param name="ex"></param>
        public FailedResult(string errorCode, string source, string errorMessage, Exception? ex = null)
        {
            ErrorCode = errorCode;
            Source = source;
            ErrorMessage = errorMessage;
            Exception = ex;
        }

        /// <inheritdoc />
        [JsonProperty]
        public bool Success => false;

        /// <summary>
        /// Exception, optional
        /// </summary>
        [JsonProperty]
        public Exception? Exception { get; private set; }

        /// <summary>
        /// Execution Errors
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<ExecutionError> Errors => errors;

        /// <summary>
        /// Add Error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(ExecutionError error)
        {
            errors.Add(error);
        }

        [JsonProperty]
        private readonly List<ExecutionError> errors = new List<ExecutionError>();
    }
}
