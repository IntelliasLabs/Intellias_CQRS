using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessReportBus : IReportBus
    {
        public Task PublishAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            return Task.CompletedTask;
        }
    }
}