namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc cref="IChildEntity" />
    public class AggregateRootTreeNode : AggregateRoot, IChildEntity
    {
        /// <summary>
        /// ParentId
        /// </summary>
        public string ParentId { get; set; }
    }
}
