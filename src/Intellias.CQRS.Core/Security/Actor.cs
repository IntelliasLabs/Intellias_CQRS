namespace Intellias.CQRS.Core.Security
{
    /// <summary>
    /// Represents an Identity who takes an action.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// Identity id.
        /// </summary>
        public string IdentityId { get; set; } = string.Empty;

        /// <summary>
        /// User id.
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
}