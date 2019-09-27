using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Store for integration events.
    /// </summary>
    public interface IIntegrationEventStore
    {
        /// <summary>
        /// Saves integration event which is not published to other subdomains.
        /// </summary>
        /// <param name="event">Integration event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveUnpublishedAsync(IIntegrationEvent @event);

        /// <summary>
        /// Marks integration event as published.
        /// </summary>
        /// <param name="event">Integration event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task MarkAsPublishedAsync(IIntegrationEvent @event);
    }
}