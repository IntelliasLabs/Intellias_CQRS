using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Tools;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// SelfProcessed EventBus
    /// </summary>
    public class SelfProcessedEventBus : IEventBus
    {
        private readonly HandlerManager _handlerManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handlerManager"></param>
        public SelfProcessedEventBus(HandlerManager handlerManager)
        {
            _handlerManager = handlerManager;
        }

        /// <summary>
        /// PublishAsync
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            var method = typeof(HandlerManager)
                    .GetMethod("HandleEventAsync");

            if (method == null)
            {
                return ExecutionResult.Fail("Error calling HandleEventAsync method");
            }

            await (Task)method
                .MakeGenericMethod(msg.GetType())
                .Invoke(_handlerManager, new object[] { msg });

            return ExecutionResult.Success;
        }
    }
}
