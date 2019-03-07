namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Any identified by ID object
    /// </summary>
    public interface IIdentified
    {
        /// <summary>
        /// Unique system message identifier
        /// </summary>
        string Id { get; }
    }
}