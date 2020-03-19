namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Format of <see cref="AzureTableValue"/>.
    /// </summary>
    public enum AzureTableValueFormat
    {
        /// <summary>
        /// Raw format. Fits to primitive types.
        /// </summary>
        Raw = 0,

        /// <summary>
        /// JSON format. Readable format for storing collections.
        /// </summary>
        Json,

        /// <summary>
        /// GZip format. Unreadable format to be used when JSON doesn't fit Azure Table limitations.
        /// </summary>
        GZip
    }
}