namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// AR abstraction
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Version of AR
        /// </summary>
        int Version { get; }
    }
}
