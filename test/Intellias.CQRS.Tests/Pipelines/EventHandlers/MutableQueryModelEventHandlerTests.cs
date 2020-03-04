using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using MediatR;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.EventHandlers
{
    public class MutableQueryModelEventHandlerTests
    {
        private readonly InProcessMutableQueryModelStorage<FakeMutableQueryModel> storage;
        private readonly FakeMediator mediator;
        private readonly DummyMutableQueryModelEventHandler handler;

        public MutableQueryModelEventHandlerTests()
        {
            storage = new InProcessMutableQueryModelStorage<FakeMutableQueryModel>(new InProcessTableStorage<FakeMutableQueryModel>());
            mediator = new FakeMediator();
            handler = new DummyMutableQueryModelEventHandler(storage, storage, mediator);
        }

        [Fact]
        public async Task HandleCreate_QueryModelDoesntExist_CreatesNew()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();
            var @event = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);
            var notification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(@event);

            // No query model in storage.
            (await storage.FindAsync(@event.SnapshotId.EntryId)).Should().BeNull();

            await handler.Handle(notification, CancellationToken.None);

            // Query model is created.
            var queryModel = await storage.GetAsync(@event.SnapshotId.EntryId);

            queryModel.Should().NotBeNull();
            queryModel.AppliedEvent.Should().BeEquivalentTo(new AppliedEvent { Id = @event.Id, Created = @event.Created });

            // Query model created notification is fired.
            var expectedSignal = QueryModelChangedSignal.CreateFromSource(
                @event,
                queryModel.Id,
                queryModel.GetType(),
                QueryModelChangeOperation.Create);

            mediator.PublishedNotifications.Single().Should().BeOfType<QueryModelChangedNotification>()
                .Which.Signal.Should().BeEquivalentTo(expectedSignal, options => options.ForSignal());
        }

        [Fact]
        public async Task HandleCreate_QueryModelExist_UpdatesExisting()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();

            // Creates query model.
            var createEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);
            var createNotification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(createEvent);
            await handler.Handle(createNotification, CancellationToken.None);

            // Updates query model.
            var updateEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);
            var updateNotification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(updateEvent);
            await handler.Handle(updateNotification, CancellationToken.None);

            // Get existing query model.
            var queryModel = await storage.GetAsync(createEvent.SnapshotId.EntryId);

            queryModel.Should().NotBeNull();
            queryModel.AppliedEvent.Should().BeEquivalentTo(new AppliedEvent { Id = updateEvent.Id, Created = updateEvent.Created });

            // Query model updated notification is fired.
            var expectedSignal = QueryModelChangedSignal.CreateFromSource(
                updateEvent,
                queryModel.Id,
                queryModel.GetType(),
                QueryModelChangeOperation.Update);

            mediator.PublishedNotifications[^1].Should().BeOfType<QueryModelChangedNotification>()
                .Which.Signal.Should().BeEquivalentTo(expectedSignal, options => options.ForSignal());
        }

        [Fact]
        public async Task HandleCreate_EventIsReplay_AddIsReplayToNotification()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();
            var @event = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);
            @event.IsReplay = true;
            var notification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(@event);

            await handler.Handle(notification, CancellationToken.None);

            mediator.PublishedNotifications.Single().Should().BeOfType<QueryModelChangedNotification>()
                .Which.IsReplay.Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleCreate_SetupQueryModelPrivacy_NotificationPrivacyIsCorrect(bool isPrivate)
        {
            // Setup query model privacy.
            handler.SetupIsPrivateQueryModel(isPrivate);

            var command = Fixtures.Pipelines.FakeCreateCommand();
            var @event = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);
            var notification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(@event);

            await handler.Handle(notification, CancellationToken.None);

            // Query model is created.
            var queryModel = await storage.GetAsync(@event.SnapshotId.EntryId);

            queryModel.Should().NotBeNull();
            queryModel.AppliedEvent.Should().BeEquivalentTo(new AppliedEvent { Id = @event.Id, Created = @event.Created });

            // Query model updated notification privacy is correct.
            mediator.PublishedNotifications.Single().Should().BeOfType<QueryModelChangedNotification>()
                .Which.IsPrivate.Should().Be(isPrivate);
        }

        [Fact]
        public async Task HandleCreate_EventAlreadyApplied_PublishesAlreadyAppliedNotification()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();
            var @event = Fixtures.Pipelines.FakeCreatedIntegrationEvent(command);

            // Ensure query model already exist.
            var queryModel = new FakeMutableQueryModel
            {
                Id = @event.SnapshotId.EntryId,
                AppliedEvent = new AppliedEvent { Id = @event.Id, Created = @event.Created }
            };

            await storage.CreateAsync(queryModel);

            // Fire event again.
            var notification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(@event);

            await handler.Handle(notification, CancellationToken.None);

            // Event already applied notification is fired.
            mediator.PublishedNotifications.Single().Should().BeOfType<EventAlreadyAppliedNotification>()
                .Which.QueryModelType.Should().Be(typeof(FakeMutableQueryModel));
        }

        [Fact]
        public async Task HandleDelete_QueryModelDoesntExist_DoesNothingAndPublishesSignal()
        {
            var command = Fixtures.Pipelines.FakeDeleteCommand();
            var @event = Fixtures.Pipelines.FakeDeletedIntegrationEvent(command);
            var notification = new IntegrationEventNotification<FakeDeletedIntegrationEvent>(@event);

            await handler.Handle(notification, CancellationToken.None);

            // Query model deleted notification is fired.
            var expectedSignal = QueryModelChangedSignal.CreateFromSource(
                @event,
                command.AggregateRootId,
                typeof(FakeMutableQueryModel),
                QueryModelChangeOperation.Delete);

            mediator.PublishedNotifications.Single().Should().BeOfType<QueryModelChangedNotification>()
                .Which.Signal.Should().BeEquivalentTo(expectedSignal, options => options.ForSignal());
        }

        [Fact]
        public async Task HandleDelete_QueryModelExists_DeletesAndPublishesSignal()
        {
            // Create query model.
            var createCommand = Fixtures.Pipelines.FakeCreateCommand();
            var createdEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent(createCommand);
            var createdNotification = new IntegrationEventNotification<FakeCreatedIntegrationEvent>(createdEvent);
            await handler.Handle(createdNotification, CancellationToken.None);

            // Query model is created.
            (await storage.FindAsync(createCommand.AggregateRootId)).Should().NotBeNull();

            // Delete query model.
            var deleteCommand = Fixtures.Pipelines.FakeDeleteCommand(new CommandSeed<FakeDeleteCommand> { AggregateRootId = createCommand.AggregateRootId });
            var deletedEvent = Fixtures.Pipelines.FakeDeletedIntegrationEvent(deleteCommand);
            var deletedNotification = new IntegrationEventNotification<FakeDeletedIntegrationEvent>(deletedEvent);
            await handler.Handle(deletedNotification, CancellationToken.None);

            // Query model is deleted.
            (await storage.FindAsync(createCommand.AggregateRootId)).Should().BeNull();

            // Query model deleted notification is fired.
            var expectedSignal = QueryModelChangedSignal.CreateFromSource(
                deletedEvent,
                createCommand.AggregateRootId,
                typeof(FakeMutableQueryModel),
                QueryModelChangeOperation.Delete);

            mediator.PublishedNotifications[^1].Should().BeOfType<QueryModelChangedNotification>()
                .Which.Signal.Should().BeEquivalentTo(expectedSignal, options => options.ForSignal());
        }

        private class DummyMutableQueryModelEventHandler : MutableQueryModelEventHandler<FakeMutableQueryModel>,
            INotificationHandler<IntegrationEventNotification<FakeCreatedIntegrationEvent>>,
            INotificationHandler<IntegrationEventNotification<FakeDeletedIntegrationEvent>>
        {
            private bool isPrivateQueryModel;

            public DummyMutableQueryModelEventHandler(
                IMutableQueryModelReader<FakeMutableQueryModel> reader,
                IMutableQueryModelWriter<FakeMutableQueryModel> writer,
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
                return HandleAsync(notification, e => e.SnapshotId.EntryId, (e, qm) =>
                {
                    qm.Data = e.Data;
                });
            }

            public Task Handle(IntegrationEventNotification<FakeDeletedIntegrationEvent> notification, CancellationToken cancellationToken)
            {
                return DeleteAsync(notification, e => e.AggregateRootId);
            }
        }
    }
}