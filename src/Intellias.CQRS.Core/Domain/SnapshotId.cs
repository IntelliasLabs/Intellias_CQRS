namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Identifier of the entry's snapshot (entry could be aggregate, query model, etc.) .
    /// </summary>
    public class SnapshotId
    {
        /// <summary>
        /// Entry id.
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// Entry version.
        /// </summary>
        public int EntryVersion { get; set; }
    }
}