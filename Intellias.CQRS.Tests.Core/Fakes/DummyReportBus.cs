using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Dummy ReportBus
    /// </summary>
    public class DummyReportBus : IReportBus
    {
        /// <summary>
        /// PublishAsync
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task<IExecutionResult> PublishAsync(IMessage msg) =>
            Task.FromResult<IExecutionResult>(ExecutionResult.Success);
    }
}
