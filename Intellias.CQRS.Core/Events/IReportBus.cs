using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <summary>
    /// Bus to report operation status to outside world (for example to presentation layer by SignalR or GraphQL Subscriptions)
    /// </summary>
    public interface IReportBus : IMessageBus<IMessage, IExecutionResult>
    {
    }
}
