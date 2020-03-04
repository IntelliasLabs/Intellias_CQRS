namespace Intellias.CQRS.Core.Signals
{
    /// <summary>
    /// Identifies an operation that changed query model.
    /// </summary>
    public enum QueryModelChangeOperation
    {
        /// <summary>
        /// Query model was created.
        /// </summary>
        Create = 0,

        /// <summary>
        /// Query model was updated.
        /// </summary>
        Update,

        /// <summary>
        /// Query model was deleted.
        /// </summary>
        Delete
    }
}