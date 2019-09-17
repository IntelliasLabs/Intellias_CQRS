using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeDispatcherEvent : IntegrationEvent
    {
        public string Data { get; set; } = string.Empty;
    }
}