using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Persistence.AzureStorage.IntegrationEvents
{
    /// <summary>
    /// Representation of stored integration event.
    /// </summary>
    public class IntegrationEventRecord
    {
        /// <summary>
        /// Integration event.
        /// </summary>
        public IIntegrationEvent IntegrationEvent { get; set; }

        /// <summary>
        /// True if integration event is published.
        /// </summary>
        public bool IsPublished { get; set; }
    }
}