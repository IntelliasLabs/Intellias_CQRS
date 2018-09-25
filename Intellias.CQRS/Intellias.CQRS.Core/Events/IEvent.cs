using Product.Domain.Core.Messages;
using System;

namespace Product.Domain.Core.Events
{
    /// <summary>
    /// Domain event interface
    /// </summary>
    public interface IEvent : IMessage
    {
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
