using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Notifications;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Fake notification bus.
    /// </summary>
    public class FakeNotificationBus : INotificationBus
    {
        private readonly List<Notification> store = new List<Notification>();

        /// <summary>
        /// Publisher notifications.
        /// </summary>
        public IReadOnlyCollection<Notification> Notifications => store.AsReadOnly();

        /// <inheritdoc/>
        public Task<IExecutionResult> PublishAsync(Notification msg)
        {
            store.Add(msg);

            return Task.FromResult<IExecutionResult>(new SuccessfulResult());
        }
    }
}
