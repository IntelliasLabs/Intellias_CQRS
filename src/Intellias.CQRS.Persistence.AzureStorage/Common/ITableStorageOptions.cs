using System;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Azure Storage Table options.
    /// </summary>
    public interface ITableStorageOptions
    {
        /// <summary>
        /// Prefix for Table name.
        /// </summary>
        string TableNamePrefix { get; }

        /// <summary>
        /// Connection string to Storage Account.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Returns partition key from id.
        /// </summary>
        Func<string, string> GetPartitionKey { get; }
    }
}