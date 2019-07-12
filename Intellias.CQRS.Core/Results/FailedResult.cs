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
        /// <param name="errorMessage"></param>
        public FailedResult(string errorMessage) : this(ErrorCodes.UnhandledError, errorMessage)
        {
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage">Error Message</param>
        public FailedResult(string errorCode, string errorMessage) : this(errorCode, string.Empty, errorMessage)
        {
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="source"></param>
        /// <param name="errorMessage">Error Message</param>
        public FailedResult(string errorCode, string source, string errorMessage)
        {
            ErrorCode = errorCode;
            Source = source;
            ErrorMessage = errorMessage;
        }

        /// <inheritdoc />
        [JsonProperty]
        public bool Success => false;

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
