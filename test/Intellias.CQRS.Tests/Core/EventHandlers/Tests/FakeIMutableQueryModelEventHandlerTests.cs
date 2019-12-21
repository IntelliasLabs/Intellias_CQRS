using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using MediatR;
using Moq;
using Xunit;

namespace Intellias.CQRS.Tests.Core.EventHandlers.Tests
{
    public class FakeIMutableQueryModelEventHandlerTests : ImmutableQueryModelEventHandlerTestsv2<FakeImmutableEventHandler, FakeImmutableQueryModel>
    {
        public FakeIMutableQueryModelEventHandlerTests()
        {
            var storage = new InProcessImmutableQueryModelStoragev2<FakeImmutableQueryModel>(new InProcessTableStorage<FakeImmutableQueryModel>());
            var mediator = new Mock<IMediator>();

            Fixture = new Fixture();
            Storage = storage;
            EventHandler = new FakeImmutableEventHandler(storage, storage, mediator.Object);
        }

        protected override Fixture Fixture { get; }

        protected override InProcessImmutableQueryModelStoragev2<FakeImmutableQueryModel> Storage { get; }

        protected override FakeImmutableEventHandler EventHandler { get; }

        [Fact]
        public async Task Handle_FakeCreated_QueryModelCreated()
        {
            await TestHandleAsync<FakeImmutableCreatedIntegrationEvent>(e => e.SnapshotId, e =>
            {
                var expectedQueryModel = new FakeImmutableQueryModel
                {
                    Id = e.SnapshotId.EntryId,
                    Version = e.SnapshotId.EntryVersion,
                    Data = e.Data,
                    AppliedEvent = new AppliedEvent(e.Id, e.Created),
                };

                return Task.FromResult(expectedQueryModel);
            });
        }

        [Fact]
        public async Task Handle_FakeUpdated_QueryModelUpdated()
        {
            await TestHandleAsync<FakeImmutableUpdatedIntegrationEvent>(e => e.SnapshotId, async e =>
            {
                var expectedQueryModel = await SetupQueryModelAsync(e, ie => e.SnapshotId);

                expectedQueryModel.Data = e.Data;

                return expectedQueryModel;
            });
        }
    }

    public class FakeImmutableEventHandler :
        ImmutableQueryModelEventHandlerv2<FakeImmutableQueryModel>,
        INotificationHandler<IntegrationEventNotification<FakeImmutableCreatedIntegrationEvent>>,
        INotificationHandler<IntegrationEventNotification<FakeImmutableUpdatedIntegrationEvent>>
    {
        public FakeImmutableEventHandler(
            CQRS.Core.Queries.Immutable.Interfaces.IImmutableQueryModelReader<FakeImmutableQueryModel> reader,
            CQRS.Core.Queries.Immutable.Interfaces.IImmutableQueryModelWriter<FakeImmutableQueryModel> writer,
            IMediator mediator)
            : base(reader, writer, mediator)
        {
        }

        public Task Handle(IntegrationEventNotification<FakeImmutableCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
        {
            return HandleAsync(notification, e => e.SnapshotId, (e, qm) =>
            {
                qm.Data = e.Data;
            });
        }

        public Task Handle(IntegrationEventNotification<FakeImmutableUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
        {
            return HandleAsync(notification, e => e.SnapshotId, (e, qm) =>
            {
                qm.Data = e.Data;
            });
        }
    }

    public class FakeImmutableQueryModel : BaseImmutableQueryModel
    {
        public SnapshotId SnapshotId { get; set; }

        public string Data { get; set; }
    }

    public class FakeImmutableCreatedIntegrationEvent : IntegrationEvent
    {
        public SnapshotId SnapshotId { get; set; }

        public string Data { get; set; }
    }

    public class FakeImmutableUpdatedIntegrationEvent : IntegrationEvent
    {
        public SnapshotId SnapshotId { get; set; }

        public string Data { get; set; }
    }
}
