using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// User configuration.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserConfig
    {
        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
