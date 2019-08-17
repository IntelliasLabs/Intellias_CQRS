using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// SelfProcessed EventBus.
    /// </summary>
    public class SelfProcessedEventBus : IEventBus
    {
        private readonly HandlerManager handlerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfProcessedEventBus"/> class.
        /// </summary>
        /// <param name="handlerManager">Handle Manager.</param>
        public SelfProcessedEventBus(HandlerManager handlerManager)
        {
            this.handlerManager = handlerManager;
        }

        /// <summary>
        /// PublishAsync.
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <returns>Execution Result.</returns>
        public async Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            try
            {
                var method = typeof(HandlerManager)
                        .GetMethod("HandleEventAsync");

                if (method == null)
                {
                    return new FailedResult("Error calling HandleEventAsync method");
                }

                await (Task)method
                    .MakeGenericMethod(msg.GetType())
                    .Invoke(handlerManager, new object[] { msg });

                return new SuccessfulResult();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                return new FailedResult($"Error handling message: {e.Message}");
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
