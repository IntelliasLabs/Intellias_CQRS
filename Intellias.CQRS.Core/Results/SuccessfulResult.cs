using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// Successful Execution Result
    /// </summary>
    public sealed class SuccessfulResult : IExecutionResult
    {
        /// <inheritdoc />
        [JsonProperty]
        public bool Success => true;
    }
}
