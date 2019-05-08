namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Extension helpers for Metadata operations
    /// </summary>
    public static class MetadataExtensions
    {
        /// <summary>
        /// Adds metadata keys from one AbstractMessage to another
        /// </summary>
        /// <param name="to">Abstract message that executes the function</param>
        /// <param name="from">From which we want to assign all MetadataKeys except MetadataKey.TypeName</param>
        /// <returns></returns>
        public static void CopyMetadataFrom(this AbstractMessage to, AbstractMessage from)
        {
            foreach (var key in from.Metadata.Keys)
            {
                to.Metadata[key] = from.Metadata[key];
            }
        }
    }
}
