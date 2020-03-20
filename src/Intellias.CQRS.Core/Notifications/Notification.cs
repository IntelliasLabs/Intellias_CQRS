using System.Diagnostics.CodeAnalysis;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Notifications
{
    /// <summary>
    /// Base class for notification.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class Notification : AbstractMessage
    {
        /// <summary>
        /// Notification recipient id.
        /// </summary>
        public string RecipientId { get; set; }
    }
}