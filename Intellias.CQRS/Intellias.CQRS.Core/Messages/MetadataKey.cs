namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Metadata key
    /// </summary>
    public enum MetadataKey
    {
        /// <summary>
        /// TypeName
        /// </summary>
        TypeName,

        /// <summary>
        /// User roles metadata
        /// </summary>
        Roles,

        /// <summary>
        /// HTTP request headers of command
        /// </summary>
        RequestHeaders,
    }
}
