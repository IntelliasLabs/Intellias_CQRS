namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Result of handler execution
    /// </summary>
    public interface IExecutionResult
    {
        /// <summary>
        /// Is result successful
        /// </summary>
        bool IsSuccess { get; }
    }
}
