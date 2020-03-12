using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Intellias.CQRS.Core.DataAnnotations;

namespace Intellias.CQRS.Core.Security
{
    /// <summary>
    /// Represents a set of Claims and Permissions under which code is executed.
    /// </summary>
    public class Principal
    {
        /// <summary>
        /// Identity id.
        /// </summary>
        [Required]
        [NotEmpty]
        public string Id { get; set; }

        /// <summary>
        /// User id.
        /// </summary>
        [Required]
        [NotEmpty]
        public string UserId { get; set; }

        /// <summary>
        /// Identity claims.
        /// </summary>
        [Required]
        public IdentityClaim[] Claims { get; set; } = Array.Empty<IdentityClaim>();

        /// <summary>
        /// Identity permissions.
        /// </summary>
        [Required]
        public string[] Permissions { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Checks whether Identity has the role.
        /// </summary>
        /// <param name="roleName">Role name.</param>
        /// <returns>True if Identity has the role.</returns>
        public bool IsInRole(string roleName)
        {
            var roleClaim = Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role && string.Equals(c.Value, roleName, StringComparison.OrdinalIgnoreCase));
            return roleClaim != null;
        }
    }
}