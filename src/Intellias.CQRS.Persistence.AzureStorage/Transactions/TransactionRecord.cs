using System.Collections.Generic;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Persistence.AzureStorage.Transactions
{
    /// <summary>
    /// Representation of command handling transaction.
    /// </summary>
    public class TransactionRecord
    {
        /// <summary>
        /// Transaction id.
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Integration event.
        /// </summary>
        public IIntegrationEvent IntegrationEvent { get; set; }

        /// <summary>
        /// State events.
        /// </summary>
        public List<IEvent> StateEvents { get; set; } = new List<IEvent>();

        /// <summary>
        /// True if transaction is committed.
        /// </summary>
        public bool IsCommitted { get; set; }
    }
}