using System;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Logger.AppInsight
{
    /// <summary>
    /// Config for Insights contexts (Operation, etc).
    /// </summary>
    public class OperationConfig
    {
        /// <summary>
        /// Operation Identifier (Correlation ID).
        /// </summary>
        public string OperationId { get; set; } = Unified.Dummy;

        /// <summary>
        /// Keeps operation name.
        /// </summary>
        public string OperationName { get; set; } = string.Empty;

        /// <summary>
        /// User AD object ID.
        /// </summary>
        public string UserId { get; set; } = Guid.Empty.ToString();
    }
}
