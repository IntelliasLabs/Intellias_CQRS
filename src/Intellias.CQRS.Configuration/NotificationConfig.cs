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
    }
}