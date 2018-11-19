namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Entity
    /// </summary>
    public interface IChildEntity
    {
        /// <summary>
        /// Id of parent entity
        /// </summary>
        string ParentId { get; }
    }
}
