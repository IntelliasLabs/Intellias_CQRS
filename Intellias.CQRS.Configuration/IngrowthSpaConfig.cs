using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// IngrowthSpaConfig
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IngrowthSpaConfig
    {
        /// <summary>
        /// Name of the origin
        /// </summary>
        public string OriginName { get; set; } = string.Empty;
    }
}
