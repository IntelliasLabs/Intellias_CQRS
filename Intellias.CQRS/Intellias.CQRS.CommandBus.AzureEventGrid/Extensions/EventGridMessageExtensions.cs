using System;
using System.Globalization;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.EventGrid.Models;

namespace Intellias.CQRS.CommandBus.AzureEventGrid.Extensions
{
    /// <summary>
    /// Converters
    /// </summary>
    public static class EventGridMessageExtensions
    {
        /// <summary>
        /// Converts command to event grid message
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static EventGridEvent ToEventGridCommand(this ICommand command) => 
            new EventGridEvent
            {
                Id = Unified.NewCode(),
                Subject = "IntelliGrowth.Command",
                EventType = command.GetType().Name,
                Data = command,
                EventTime = DateTime.Now,
                DataVersion = command.ExpectedVersion.ToString(CultureInfo.InvariantCulture)
            };
    }
}
