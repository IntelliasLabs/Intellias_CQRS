using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Utils.Pipelines.Builder;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;

namespace Intellias.CQRS.Tests.Utils.Pipelines
{
    public class PipelinesFixtures
    {
        public FakeCreateCommand FakeCreateCommand()
        {
            return FakeCreateCommand(new CommandSeed<FakeCreateCommand>());
        }

        public FakeCreateCommand FakeCreateCommand(CommandSeed<FakeCreateCommand> seed)
        {
            return Fixtures.CommandFromBuilder(f => new FakeCreateCommandBuilder(f, seed));
        }

        public FakeUpdateCommand FakeUpdateCommand()
        {
            return FakeUpdateCommand(new CommandSeed<FakeUpdateCommand>());
        }

        public FakeUpdateCommand FakeUpdateCommand(CommandSeed<FakeUpdateCommand> seed)
        {
            return Fixtures.CommandFromBuilder(f => new FakeUpdateCommandBuilder(f, seed));
        }

        public FakeDispatcherCommand FakeDispatcherCommand()
        {
            return Fixtures.CommandFromBuilder(f => new FakeDispatcherCommandBuilder(f, new CommandSeed<FakeDispatcherCommand>()));
        }

        public FakeDispatcherEvent FakeDispatcherEvent()
        {
            return Fixtures.IntegrationEvent<FakeDispatcherEvent>(FakeDispatcherCommand(), e => { });
        }

        public FakeCreatedIntegrationEvent FakeCreatedIntegrationEvent(FakeCreateCommand command)
        {
            return Fixtures.IntegrationEvent<FakeCreatedIntegrationEvent>(command, e =>
            {
                e.SnapshotId = new SnapshotId(command.AggregateRootId, 0);
                e.Data = command.Data;
            });
        }
    }
}