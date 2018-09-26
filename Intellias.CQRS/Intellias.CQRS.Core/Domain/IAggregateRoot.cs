namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
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
