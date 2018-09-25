namespace Product.Domain.Core.Domain
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
