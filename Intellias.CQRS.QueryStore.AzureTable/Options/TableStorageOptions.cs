namespace Intellias.CQRS.QueryStore.AzureTable.Options
{
    /// <summary>
    /// Table storage configuration options.
    /// </summary>
    public class TableStorageOptions
    {
        /// <summary>
        /// Number of filters by Id in a single query.
        /// Is used to query storage using multiple RowIds.
        /// Limited only by the length of URI of the built filter.
        /// </summary>
        public int QueryChunkSize { get; set; } = 50;

        /// <summary>
        /// Prefix for Table name.
        /// </summary>
        public string TableNamePrefix { get; set; } = string.Empty;

        /// <summary>
        /// Connection string to Storage Account.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;
    }
}