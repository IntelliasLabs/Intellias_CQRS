using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// Notification configuration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class NotificationConfig
    {
        /// <summary>
        /// Email configuration.
        /// </summary>
        public EmailConfig Email { get; set; } = new EmailConfig();

        /// <summary>
        /// Spa web host.
        /// </summary>
        public string SpaWebHost { get; set; } = string.Empty;

        /// <summary>
        /// Public assets host.
        /// </summary>
        public string AssetsHost { get; set; } = string.Empty;
    }
}