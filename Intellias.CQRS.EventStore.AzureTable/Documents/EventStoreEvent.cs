using System;
using System.Globalization;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.EventStore.AzureTable.Documents
{
    /// <summary>
    /// StoreEventItem
    /// </summary>
    public class EventStoreEvent : TableEntity
    {
        /// <summary>
        /// Keeps serialized event itself
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Keeps an event type
        /// </summary>
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// ctor buildes azure table store entity from IEvent
        /// </summary>
        /// <param name="event"></param>
        public EventStoreEvent(IEvent @event)
        {
            PartitionKey = @event.AggregateRootId;
            RowKey = @event.Version.ToString("D9", CultureInfo.InvariantCulture);
            Data = @event.ToJson();
            TypeName = @event.TypeName;
        }

        /// <summary>
        /// parameterless ctor
        /// </summary>
        public EventStoreEvent() { }

        /// <summary>
        /// Converts an EventStoreEvent to IEvent
        /// </summary>
        public IEvent ToEvent()
        {
            var @event = (IEvent)Data.FromJson(Type.GetType(TypeName));

            @event.Version = int.Parse(RowKey, CultureInfo.InvariantCulture);
            return @event;
        }
    }
}
