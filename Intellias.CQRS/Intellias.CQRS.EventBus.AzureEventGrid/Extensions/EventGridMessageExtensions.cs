using System;
using System.Globalization;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.EventGrid.Models;

namespace Intellias.CQRS.EventBus.AzureEventGrid.Extensions
{
    /// <summary>
    /// Converters
    /// </summary>
    public static class EventGridMessageExtensions
    {
        /// <summary>
        /// Converts command to event grid message
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public static EventGridEvent ToEventGridEvent(this IEvent @event) => 
            new EventGridEvent
            {
                Id = Unified.NewCode(),
                Subject = "IntelliGrowth.Command",
                EventType = @event.GetType().Name,
                Data = @event,
                EventTime = DateTime.Now,
                DataVersion = @event.Version.ToString(CultureInfo.InvariantCulture)
            };
    }
}
