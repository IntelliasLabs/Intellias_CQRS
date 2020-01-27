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
        /// PACE user section.
        /// </summary>
        public UserConfig PaceUser { get; set; } = new UserConfig();

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