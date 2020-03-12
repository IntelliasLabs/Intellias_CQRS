namespace Intellias.CQRS.Core.Security
{
    /// <summary>
    /// Identity claim.
    /// </summary>
    public class IdentityClaim
    {
        /// <summary>
        /// Claim type.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Claim value.
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}