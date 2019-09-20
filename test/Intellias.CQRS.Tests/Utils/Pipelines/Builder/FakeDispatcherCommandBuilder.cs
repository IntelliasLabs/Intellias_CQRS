using AutoFixture;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Builder
{
    public class FakeDispatcherCommandBuilder : CommandBuilder<FakeDispatcherCommand>
    {
        public FakeDispatcherCommandBuilder(IFixture fixture, CommandSeed<FakeDispatcherCommand> seed)
            : base(fixture, seed)
        {
        }

        protected override void Setup(FakeDispatcherCommand command)
        {
        }
    }
}