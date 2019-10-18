using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Tests.Core.EventHandlers
{
    /// <summary>
    /// Please DO NOT register this class in DI services.
    /// </summary>
    public class UnregisteredEventHandler :
        IEventHandler<TestDeletedEvent>
    {
        /// <inheritdoc />
        public Task HandleAsync(TestDeletedEvent @event) =>
            Task.CompletedTask;
    }
}
