using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// Email configuration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailConfig
    {
        /// <summary>
        /// User name.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// user password.
        /// </summary>
        public string UserPassword { get; set; } = string.Empty;

        /// <summary>
        /// SMTP host.
        /// </summary>
        public string SmtpHost { get; set; } = string.Empty;

        /// <summary>
        /// SMTP port.
        /// </summary>
        public int SmtpPort { get; set; } = 587;
    }
}