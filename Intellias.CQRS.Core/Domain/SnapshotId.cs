namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Identifier of the entry's snapshot (entry could be aggregate, query model, etc.) .
    /// </summary>
    public class SnapshotId
    {
        /// <summary>
        /// Empty <see cref="SnapshotId"/> object.
        /// </summary>
        public static readonly SnapshotId Empty = new SnapshotId(string.Empty, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotId"/> class.
        /// </summary>
        /// <param name="entryId">Value for <see cref="EntryId"/>.</param>
        /// <param name="entryVersion">Value for <see cref="EntryVersion"/>.</param>
        public SnapshotId(string entryId, int entryVersion)
        {
            EntryId = entryId;
            EntryVersion = entryVersion;
        }

        /// <summary>
        /// Entry id.
        /// </summary>
        public string EntryId { get; }

        /// <summary>
        /// Entry version.
        /// </summary>
        public int EntryVersion { get; }
    }
}