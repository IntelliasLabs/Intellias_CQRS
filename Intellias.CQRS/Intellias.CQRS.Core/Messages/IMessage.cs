using System;
using System.Collections.Generic;

namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Represents system cross-domain message
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Unique system message identifier
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Aggregate Root Identifier
        /// </summary>
        string AggregateRootId { get; set; }

        /// <summary>
        /// User/Actor identifier
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Gets the time stamp for this message creation.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point in time where this event occurred.</value>
        DateTime Created
        {
            get;
        }

        /// <summary>
        /// Message Metadata
        /// </summary>
        IDictionary<MetadataKey, string> Metadata { get; }
    }
}
