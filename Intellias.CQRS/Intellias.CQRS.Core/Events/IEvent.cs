using System;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Domain event interface
    /// </summary>
    public interface IEvent : IMessage
    {
        /// <summary>
        /// Source Aggregate Root identifier
        /// </summary>
        string AggregateRootId { get; }

        /// <summary>
        /// Version of AR that generated an event
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Gets the time stamp for this event.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point
        /// in time where this event occurred.</value>
        DateTime Created
        {
            get;
        }
    }
}
