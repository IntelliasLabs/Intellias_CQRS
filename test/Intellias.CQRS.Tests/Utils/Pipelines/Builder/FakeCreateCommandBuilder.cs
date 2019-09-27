using AutoFixture;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Builder
{
    public class FakeCreateCommandBuilder : CommandBuilder<FakeCreateCommand>
    {
        public FakeCreateCommandBuilder(IFixture fixture, CommandSeed<FakeCreateCommand> seed)
            : base(fixture, seed)
        {
        }

        protected override void Setup(FakeCreateCommand command)
        {
            command.Data = Fixture.Create<string>();
        }
    }
}