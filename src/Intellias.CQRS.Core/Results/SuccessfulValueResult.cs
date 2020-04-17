using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// Successful result that contains <see cref="Value"/>>.
    /// </summary>
    public class SuccessfulValueResult : IExecutionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulValueResult"/> class.
        /// </summary>
        /// <param name="value">Value for <see cref="Value"/>.</param>
        [JsonConstructor]
        public SuccessfulValueResult(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Result value.
        /// </summary>
        public object Value { get; }

        /// <inheritdoc />
        public bool Success { get; } = true;
    }
}