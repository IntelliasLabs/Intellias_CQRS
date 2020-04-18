using System;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Persistence.AzureStorage.Common
{
    /// <summary>
    /// Azure Storage Table options.
    /// </summary>
    public class DefaultTableStorageOptions : ITableStorageOptions
    {
        /// <inheritdoc />
        public string TableNamePrefix { get; set; }

        /// <inheritdoc />
        public string ConnectionString { get; set; }

        /// <inheritdoc />
        public Func<string, string> GetPartitionKey { get; set; } = id => Unified.Partition(id);
    }
}