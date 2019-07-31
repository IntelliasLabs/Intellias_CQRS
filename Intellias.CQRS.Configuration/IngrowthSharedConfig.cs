using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// IngrowthSharedConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IngrowthSharedConfig
    {
        /// <summary>
        /// ServiceBusConnectionString
        /// </summary>
        public string ServiceBusConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// StorageAccountConnectionString
        /// </summary>
        public string StorageAccountConnectionString { get; set; } = string.Empty;
    }
}
