using System;
using System.Globalization;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable.Documents
{
    /// <summary>
    /// StoreEventItem.
    /// </summary>
    public class EventStoreEvent : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreEvent"/> class.
        /// </summary>
        /// <param name="event">Event.</param>
        public EventStoreEvent(IEvent @event)
        {
            PartitionKey = @event.AggregateRootId;
            RowKey = @event.Version.ToString("D9", CultureInfo.InvariantCulture);
            Data = @event.ToJson();
            TypeName = @event.GetType().AssemblyQualifiedName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreEvent"/> class.
        /// </summary>
        public EventStoreEvent()
        {
        }

        /// <summary>
        /// Keeps serialized event itself.
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Keeps an event type.
        /// </summary>
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// Converts an EventStoreEvent to IEvent.
        /// </summary>
        /// <returns>Event.</returns>
        public IEvent ToEvent()
        {
            var @event = (IEvent)Data.FromJson(Type.GetType(TypeName));

            @event.Version = int.Parse(RowKey, CultureInfo.InvariantCulture);
            return @event;
        }
    }
}
