namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Represents system cross-domain message
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Unique system message identifier
        /// </summary>
        string AggregateRootId { get; }
    }
}
