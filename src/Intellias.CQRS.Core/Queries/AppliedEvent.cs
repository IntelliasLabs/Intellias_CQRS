using System;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// Last IIntegrationEvent that updated query model.
    /// </summary>
    public class AppliedEvent
    {
        /// <summary>
        /// Integration Event id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Integration Event creation timestamp.
        /// </summary>
        public DateTimeOffset Created { get; set; }
    }
}