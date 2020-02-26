using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using MediatR;
using Moq;
using Xunit;

namespace Intellias.CQRS.Tests.Core.EventHandlers.Tests
{
    public class FakeMutableQueryModelEventHandlerTests : MutableQueryModelEventHandlerTests<FakeMutableEventHandler, FakeMutableQueryModel>
    {
        public FakeMutableQueryModelEventHandlerTests()
        {
            var storage = new InProcessMutableQueryModelStorage<FakeMutableQueryModel>(new InProcessTableStorage<FakeMutableQueryModel>());
            var mediator = new Mock<IMediator>();

            Fixture = new Fixture();
            Storage = storage;
            EventHandler = new FakeMutableEventHandler(storage, storage, mediator.Object);
        }

        protected override Fixture Fixture { get; }

        protected override InProcessMutableQueryModelStorage<FakeMutableQueryModel> Storage { get; }

        protected override FakeMutableEventHandler EventHandler { get; }

        [Fact]
        public async Task Handle_FakeCreated_QueryModelCreated()
        {
            await TestHandleAsync<FakeCreatedIntegrationEvent>(e => e.Id, e =>
            {
                var expectedQueryModel = new FakeMutableQueryModel
                {
                    Id = e.Id,
                    Data = e.Data,
                    AppliedEvent = new AppliedEvent { Id = e.Id, Created = e.Created }
                };

                return Task.FromResult(expectedQueryModel);
            });
        }

        [Fact]
        public async Task Handle_FakeUpdated_QueryModelUpdated()
        {
            await TestHandleAsync<FakeUpdatedIntegrationEvent>(e => e.Id, async e =>
            {
                var expectedQueryModel = await SetupQueryModelAsync(e, ie => e.Id);

                expectedQueryModel.Data = e.Data;

                return expectedQueryModel;
            });
        }
    }

    public class FakeMutableEventHandler :
        MutableQueryModelEventHandler<FakeMutableQueryModel>,
        INotificationHandler<IntegrationEventNotification<FakeCreatedIntegrationEvent>>,
        INotificationHandler<IntegrationEventNotification<FakeUpdatedIntegrationEvent>>
    {
        public FakeMutableEventHandler(
            IMutableQueryModelReader<FakeMutableQueryModel> reader,
            IMutableQueryModelWriter<FakeMutableQueryModel> writer,
            IMediator mediator)
            : base(reader, writer, mediator)
        {
        }

        public Task Handle(IntegrationEventNotification<FakeCreatedIntegrationEvent> notification, CancellationToken cancellationToken)
        {
            return HandleAsync(notification, e => e.Id, (e, qm) =>
            {
                qm.Data = e.Data;
            });
        }

        public Task Handle(IntegrationEventNotification<FakeUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
        {
            return HandleAsync(notification, e => e.Id, (e, qm) =>
            {
                qm.Data = e.Data;
            });
        }
    }

    public class FakeMutableQueryModel : BaseMutableQueryModel
    {
        public string Data { get; set; }
    }

    public class FakeCreatedIntegrationEvent : IntegrationEvent
    {
        public string Data { get; set; }
    }

    public class FakeUpdatedIntegrationEvent : IntegrationEvent
    {
        public string Data { get; set; }
    }
}
