using Intellias.CQRS.Core.Signals;
using MediatR;

namespace Intellias.CQRS.Pipelines.EventHandlers.Notifications
{
    /// <summary>
    /// Query model changed notification.
    /// </summary>
    public class QueryModelChangedNotification : INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModelChangedNotification"/> class.
        /// </summary>
        /// <param name="signal">Value for <see cref="Signal"/>.</param>
        public QueryModelChangedNotification(QueryModelChangedSignal signal)
        {
            Signal = signal;
        }

        /// <summary>
        /// Query model changed signal.
        /// </summary>
        public QueryModelChangedSignal Signal { get; }

        /// <summary>
        /// Indicates whether notification source is replay.
        /// </summary>
        public bool IsReplay { get; set; }

        /// <summary>
        /// Indicates whether notification is private, thus shouldn't be published.
        /// </summary>
        public bool IsPrivate { get; set; }
    }
}