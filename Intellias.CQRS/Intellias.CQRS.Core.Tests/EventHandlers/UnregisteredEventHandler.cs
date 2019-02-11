using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Events;

namespace Intellias.CQRS.Core.Tests.EventHandlers
{
    /// <summary>
    /// Please DO NOT register this class in DI services
    /// </summary>
    public class UnregisteredEventHandler :
        IEventHandler<TestDeletedEvent>
    {
        /// <summary>
        /// HandleAsync TestDeletedEvent
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task<IExecutionResult> HandleAsync(TestDeletedEvent @event) =>
            await Task.FromResult(ExecutionResult.Success);
    }
}
