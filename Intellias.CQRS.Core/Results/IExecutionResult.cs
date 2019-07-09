namespace Intellias.CQRS.Core.Results
{
    /// <summary>
    /// Execution Result
    /// </summary>
    public interface IExecutionResult
    {
        /// <summary>
        /// Is result successful
        /// </summary>
        bool Success { get; }
    }
}
