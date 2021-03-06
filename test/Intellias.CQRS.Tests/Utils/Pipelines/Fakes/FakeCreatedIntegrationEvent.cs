using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeCreatedIntegrationEvent : IntegrationEvent
    {
        public SnapshotId SnapshotId { get; set; }

        public string Data { get; set; } = string.Empty;
    }
}