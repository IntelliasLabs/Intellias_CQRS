using System.Linq;

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
            var namesWithoutTypeName = from.Metadata.Keys
                .Where(e => e != MetadataKey.TypeName);

            foreach (var key in namesWithoutTypeName)
            {
                to.Metadata[key] = from.Metadata[key];
            }
        }
    }
}
