using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using MediatR;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.EventHandlers
{
    public class ImmutableQueryModelEventHandlerTests
    {
        private readonly InProcessImmutableQueryModelStorage<FakeImmutableQueryModel> storage;
        private readonly FakeMediator mediator;
        private readonly DummyImmutableQueryModelEventHandler handler;

        public ImmutableQueryModelEventHandlerTests()
        {
            storage = new InProcessImmutableQueryModelStorage<FakeImmutableQueryModel>(new InProcessTableStorage<FakeImmutableQueryModel>());
            mediator = new FakeMediator();
            handler = new DummyImmutableQueryModelEventHandler(storage, storage, mediator);
        }

        [Fact]
        public async Task HandleAsync_QueryModelDoesntExist_CreatesNew()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();
            var @event = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);
            var notification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(@event);

            // No query model in storage.
            (await storage.FindAsync(@event.SnapshotId.EntryId, @event.SnapshotId.EntryVersion)).Should().BeNull();

            await handler.Handle(notification, CancellationToken.None);

            // Query model is created.
            var queryModel = await storage.GetAsync(@event.SnapshotId.EntryId, @event.SnapshotId.EntryVersion);

            queryModel.Should().NotBeNull();
            queryModel.AppliedEvent.Should().BeEquivalentTo(new AppliedEvent { Id = @event.Id, Created = @event.Created });

            // Query model updated notification is fired.
            mediator.PublishedNotifications.Single().Should().BeOfType<QueryModelUpdatedNotification>()
                .Which.Signal.QueryModelId.Should().BeEquivalentTo(queryModel.Id);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_SetupQueryModelPrivacy_NotificationPrivacyIsCorrect(bool isPrivate)
        {
            // Setup query model privacy.
            handler.SetupIsPrivateQueryModel(isPrivate);

            var command = Fixtures.Pipelines.FakeCreateCommand();
            var @event = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);
            var notification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(@event);

            await handler.Handle(notification, CancellationToken.None);

            // Query model is created.
            var queryModel = await storage.GetAsync(@event.SnapshotId.EntryId, @event.SnapshotId.EntryVersion);

            queryModel.Should().NotBeNull();
            queryModel.AppliedEvent.Should().BeEquivalentTo(new AppliedEvent { Id = @event.Id, Created = @event.Created });

            // Query model updated notification privacy is correct.
            mediator.PublishedNotifications.Single().Should().BeOfType<QueryModelUpdatedNotification>()
                .Which.IsPrivate.Should().Be(isPrivate);
        }

        [Fact]
        public async Task HandleAsync_EventAlreadyApplied_PublishesAlreadyAppliedNotification()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();
            var @event = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);

            // Ensure query model already exist.
            var queryModel = new FakeImmutableQueryModel
            {
                Id = @event.SnapshotId.EntryId,
                Version = @event.SnapshotId.EntryVersion,
                AppliedEvent = new AppliedEvent { Id = @event.Id, Created = @event.Created }
            };

            await storage.CreateAsync(queryModel);

            // Fire event again.
            var notification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(@event);

            await handler.Handle(notification, CancellationToken.None);

            // Event already applied notification is fired.
            mediator.PublishedNotifications.Single().Should().BeOfType<EventAlreadyAppliedNotification>()
                .Which.QueryModelType.Should().Be(typeof(FakeImmutableQueryModel));
        }

        private class DummyImmutableQueryModelEventHandler : ImmutableQueryModelEventHandler<FakeImmutableQueryModel>,
            INotificationHandler<IntegrationEventNotification<FakeCreatedIntegrationEvent>>
        {
            private bool isPrivateQueryModel;

            public DummyImmutableQueryModelEventHandler(
                IImmutableQueryModelReader<FakeImmutableQueryModel> reader,
                IImmutableQueryModelWriter<FakeImmutableQueryModel> writer,
                IMediator mediator)
                : base(reader, writer, mediator)
            {
            }

            protected override bool IsPrivateQueryModel => isPrivateQueryModel;

            public void SetupIsPrivateQueryModel(bool value)
            {
                isPrivateQueryModel = value;
            }

            public Task Handle(IntegrationEventNotification<FakeCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
            {
                return HandleAsync(notification, e => e.SnapshotId, (e, qm) =>
                {
                    qm.Data = e.Data;
                });
            }
        }
    }
}