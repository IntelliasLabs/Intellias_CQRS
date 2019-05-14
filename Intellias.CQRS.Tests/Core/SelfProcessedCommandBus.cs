using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Tools;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Self Processed Command Bus used for process manager needs
    /// </summary>
    public class SelfProcessedCommandBus : ICommandBus
    {
        private readonly HandlerManager _handlerManager;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="handlerManager"></param>
        public SelfProcessedCommandBus(HandlerManager handlerManager)
        {
            _handlerManager = handlerManager;
        }

        /// <summary>
        /// PublishAsync ICommand
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<IExecutionResult> PublishAsync(ICommand msg)
        {
            var method = typeof(HandlerManager)
                    .GetMethod("HandleCommandAsync");

            if (method == null)
            {
                return ExecutionResult.Fail("Error calling HandleCommandAsync method");
            }

            await (Task)method
                .MakeGenericMethod(msg.GetType())
                .Invoke(_handlerManager, new object[] { msg });

            return ExecutionResult.Success;
        }
    }
}
