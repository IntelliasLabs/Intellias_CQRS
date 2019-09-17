using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Builder;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;

namespace IntelliGrowth.JobProfiles.Tests.Utils.Pipelines
{
    public class PipelinesFixtures
    {
        public FakeDispatcherCommand FakeDispatcherCommand()
        {
            return Fixtures.CommandFromBuilder(f => new FakeDispatcherCommandBuilder(f, new CommandSeed<FakeDispatcherCommand>()));
        }

        public FakeDispatcherEvent FakeDispatcherEvent()
        {
            return Fixtures.IntegrationEvent<FakeDispatcherEvent>(FakeDispatcherCommand(), e => { });
        }
    }
}