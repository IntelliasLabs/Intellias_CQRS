using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Extensions
{
    /// <summary>
    /// Event extension methods collection
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Converts some ProcessEvent type to another
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public static TEvent ToType<TEvent>(this ProcessEvent @event)
            where TEvent : ProcessEvent, new()
        {
            var result = ((AbstractMessage)@event).ToType<TEvent>();

            result.SourceId = @event.SourceId;
            result.Version = @event.Version;
            result.Process = @event.Process;

            return result;
        }
    }
}
