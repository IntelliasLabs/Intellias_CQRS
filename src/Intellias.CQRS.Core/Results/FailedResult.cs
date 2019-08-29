using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// Failed Execution Result.
    /// </summary>
    public sealed class FailedResult : ExecutionError, IExecutionResult
    {
        [JsonProperty]
        private readonly List<ExecutionError> details = new List<ExecutionError>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedResult"/> class.
        /// Failed Result.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public FailedResult(string message)
            : this(ErrorCodes.UnhandledError, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedResult"/> class.
        /// Execution Error.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <param name="message">Error Message.</param>
        public FailedResult(string code, string message)
            : this(code, string.Empty, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedResult"/> class.
        /// Execution Error.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <param name="source">Source.</param>
        /// <param name="message">Error Message.</param>
        public FailedResult(string code, string source, string message)
        {
            Code = code;
            Source = source;
            Message = message;
        }

        [JsonConstructor]
        private FailedResult()
        {
        }

        /// <inheritdoc />
        [JsonProperty]
        public bool Success => false;

        /// <summary>
        /// Execution Errors.
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<ExecutionError> Details => details;

        /// <summary>
        /// Add Error.
        /// </summary>
        /// <param name="errorDetail">Error.</param>
        public void AddError(ExecutionError errorDetail)
        {
            details.Add(errorDetail);
        }
    }
}
