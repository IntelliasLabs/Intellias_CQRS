using System;
using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// IngrowthIdentityConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IngrowthIdentityConfig
    {
        /// <summary>
        /// CoE Groups Root-Node Id
        /// </summary>
        public Guid CoEGroupsRootNodeId { get; set; }

        /// <summary>
        /// StorageAccountConnectionString
        /// </summary>
        public string StorageAccountConnectionString { get; set; } = string.Empty;
    }
}
