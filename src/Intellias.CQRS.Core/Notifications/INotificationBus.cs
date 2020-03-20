using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Notifications
{
    /// <summary>
    /// Abstraction of notification bus.
    /// </summary>
    public interface INotificationBus : IMessageBus<Notification>
    {
    }
}