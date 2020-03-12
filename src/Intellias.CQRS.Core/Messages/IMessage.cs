using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Security;

namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Represents system cross-domain message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Unique system message identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Id of operation (Operation can consist of several commands / events).
        /// Used for operations tracking or operation rejecting for example.
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// Aggregate Root Identifier.
        /// </summary>
        string AggregateRootId { get; set; }

        /// <summary>
        /// Gets the time stamp for this message creation.
        /// </summary>
        /// <value>a <see cref="DateTime"/> UTC value that represents the point in time where this event occurred.</value>
        DateTime Created { get; }

        /// <summary>
        /// Contains a security context in which message is handled.
        /// </summary>
        Principal Principal { get; }

        /// <summary>
        /// Message Metadata.
        /// </summary>
        IDictionary<MetadataKey, string> Metadata { get; }
    }
}
