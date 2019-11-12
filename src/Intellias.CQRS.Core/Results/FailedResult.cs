using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Results.Errors;
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
        /// </summary>
        /// <param name="errorCodeInfo">Error Code Info.</param>
        public FailedResult(ErrorCodeInfo errorCodeInfo)
            : base(errorCodeInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedResult"/> class.
        /// </summary>
        /// <param name="errorCodeInfo">Error Code Info.</param>
        /// <param name="source">Source.</param>
        public FailedResult(ErrorCodeInfo errorCodeInfo, string source)
            : base(errorCodeInfo, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedResult"/> class.
        /// </summary>
        /// <param name="errorCodeInfo">Error Code Info.</param>
        /// <param name="source">Source.</param>
        /// <param name="customMessage">Custom message.</param>
        public FailedResult(ErrorCodeInfo errorCodeInfo, string source, string customMessage)
            : base(errorCodeInfo, source, customMessage)
        {
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
        /// Builds Failed Result using command property pass and default message.
        /// </summary>
        /// <param name="externalErrorCodeInfo">External Error Code Info.</param>
        /// <param name="detailsCodeInfo">Internal Error Code Info.</param>
        /// <returns>FailedResult.</returns>
        [Obsolete("Please use alternative method name")]
        public static FailedResult CreateWithInternal(ErrorCodeInfo externalErrorCodeInfo, ErrorCodeInfo detailsCodeInfo)
        {
            return Create(externalErrorCodeInfo, detailsCodeInfo, detailsCodeInfo.Message);
        }

        /// <summary>
        /// Builds Failed Result using command property pass and custom message.
        /// </summary>
        /// <param name="externalErrorCodeInfo">External Error Code Info.</param>
        /// <param name="detailsCodeInfo">Internal Error Code Info.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>FailedResult.</returns>
        [Obsolete("Please use alternative method name")]
        public static FailedResult CreateWithInternal(
            ErrorCodeInfo externalErrorCodeInfo,
            ErrorCodeInfo detailsCodeInfo,
            string errorMessage)
        {
            var result = new FailedResult(externalErrorCodeInfo);

            var internalError = new ExecutionError(detailsCodeInfo, null, errorMessage);

            result.AddError(internalError);

            return result;
        }

        /// <summary>
        /// Builds Validation Failed Result with all passed internal errors.
        /// </summary>
        /// <param name="internalErrors">Internal Execution Errors.</param>
        /// <returns>FailedResult.</returns>
        [Obsolete("Please use alternative method name")]
        public static FailedResult ValidationFailedWith(IReadOnlyCollection<ExecutionError> internalErrors)
        {
            var result = new FailedResult(CoreErrorCodes.ValidationFailed);

            foreach (var internalError in internalErrors)
            {
                result.AddError(internalError);
            }

            return result;
        }

        /// <summary>
        /// Builds Failed Result using command property pass and default message.
        /// </summary>
        /// <param name="externalErrorCodeInfo">External Error Code Info.</param>
        /// <param name="detailsCodeInfo">Internal Error Code Info.</param>
        /// <returns>FailedResult.</returns>
        public static FailedResult Create(ErrorCodeInfo externalErrorCodeInfo, ErrorCodeInfo detailsCodeInfo)
        {
            return Create(externalErrorCodeInfo, detailsCodeInfo, detailsCodeInfo.Message);
        }

        /// <summary>
        /// Builds Failed Result using command property pass and custom message.
        /// </summary>
        /// <param name="externalErrorCodeInfo">External Error Code Info.</param>
        /// <param name="detailsCodeInfo">Internal Error Code Info.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>FailedResult.</returns>
        public static FailedResult Create(
            ErrorCodeInfo externalErrorCodeInfo,
            ErrorCodeInfo detailsCodeInfo,
            string errorMessage)
        {
            var result = new FailedResult(externalErrorCodeInfo);

            var internalError = new ExecutionError(detailsCodeInfo, null, errorMessage);

            result.AddError(internalError);

            return result;
        }

        /// <summary>
        /// Builds Validation Failed Result with all passed internal errors.
        /// </summary>
        /// <param name="internalErrors">Internal Execution Errors.</param>
        /// <returns>FailedResult.</returns>
        public static FailedResult ValidationFailed(IReadOnlyCollection<ExecutionError> internalErrors)
        {
            var result = new FailedResult(CoreErrorCodes.ValidationFailed);

            foreach (var internalError in internalErrors)
            {
                result.AddError(internalError);
            }

            return result;
        }

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
