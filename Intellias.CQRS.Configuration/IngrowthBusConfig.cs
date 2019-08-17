using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// IngrowthBusConfig.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IngrowthBusConfig
    {
        /// <summary>
        /// StorageAccountConnectionString.
        /// </summary>
        public string StorageAccountConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// CommandBusTopic.
        /// </summary>
        public string CommandBusTopic { get; set; } = string.Empty;

        /// <summary>
        /// EventBusTopic.
        /// </summary>
        public string EventBusTopic { get; set; } = string.Empty;
    }
}
