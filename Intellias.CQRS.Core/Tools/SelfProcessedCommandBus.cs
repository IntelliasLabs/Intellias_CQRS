using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Self Processed Command Bus used for process manager needs.
    /// </summary>
    public class SelfProcessedCommandBus : ICommandBus
    {
        private readonly HandlerManager handlerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfProcessedCommandBus"/> class.
        /// </summary>
        /// <param name="handlerManager">Handler Manager.</param>
        public SelfProcessedCommandBus(HandlerManager handlerManager)
        {
            this.handlerManager = handlerManager;
        }

        /// <summary>
        /// PublishAsync ICommand.
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <returns>Execution Result.</returns>
        public async Task<IExecutionResult> PublishAsync(ICommand msg)
        {
            try
            {
                var method = typeof(HandlerManager)
                        .GetMethod("HandleCommandAsync");

                if (method == null)
                {
                    return new FailedResult("Error calling HandleCommandAsync method");
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
