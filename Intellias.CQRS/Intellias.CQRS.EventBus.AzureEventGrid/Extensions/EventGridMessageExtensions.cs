using System;
using System.Globalization;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventBus.AzureEventGrid.Extensions
{
    /// <summary>
    /// Converters
    /// </summary>
    public static class EventGridMessageExtensions
    {
        /// <summary>
        /// Converts event to event grid message
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventGridEvent ToEventGridEvent(this IEvent @event) => 
            new EventGridEvent
            {
                Id = Unified.NewCode(),
                Subject = "IntelliGrowth.Event",
                EventType = @event.GetType().Name,
                Data = JsonConvert.SerializeObject(@event, CqrsSettings.JsonConfig()),
                EventTime = DateTime.Now,
                DataVersion = @event.Version.ToString(CultureInfo.InvariantCulture)
            };
    }
}
