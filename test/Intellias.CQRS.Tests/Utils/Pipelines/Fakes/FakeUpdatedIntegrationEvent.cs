using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeUpdatedIntegrationEvent : IntegrationEvent
    {
        public string Data { get; set; }
    }
}