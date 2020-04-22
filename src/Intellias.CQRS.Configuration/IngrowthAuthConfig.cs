using System;
using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Configuration
{
    /// <summary>
    /// IngrowthAuthConfig.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IngrowthAuthConfig
    {
        /// <summary>
        /// ApiClientId.
        /// </summary>
        public Guid ApiClientId { get; set; }

        /// <summary>
        /// ApiSwaggerClientId.
        /// </summary>
        public Guid ApiSwaggerClientId { get; set; }

        /// <summary>
        /// ApiSwaggerClientSecret.
        /// </summary>
        public string ApiSwaggerClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// SpaClientId.
        /// </summary>
        public string SpaClientId { get; set; } = string.Empty;

        /// <summary>
        /// SpaClientId.
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// MsGraphClientId.
        /// </summary>
        public Guid MsGraphClientId { get; set; }

        /// <summary>
        /// MsGraphClientSecret.
        /// </summary>
        public string MsGraphClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// JIRA server host URI.
        /// </summary>
        public Uri JiraHost { get; set; }

        /// <summary>
        /// JIRA server user login.
        /// </summary>
        public string JiraLogin { get; set; } = string.Empty;

        /// <summary>
        /// JIRA server user password.
        /// </summary>
        public string JiraPassword { get; set; } = string.Empty;
    }
}
