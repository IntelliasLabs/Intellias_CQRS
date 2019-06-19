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
        /// <param name="message">message</param>
        /// <returns></returns>
        public Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessage =>
            Task.CompletedTask;
    }
}
