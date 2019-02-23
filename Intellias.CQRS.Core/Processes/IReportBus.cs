using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Processes
{
    /// <summary>
    /// Message-bus to report operation status to outside world
    /// (for example to presentation layer by SignalR)
    /// </summary>
    public interface IReportBus : IEventBus
    { }
}
