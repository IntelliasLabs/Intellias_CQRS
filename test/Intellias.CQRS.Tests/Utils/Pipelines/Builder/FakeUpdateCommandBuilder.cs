using AutoFixture;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Builder
{
    public class FakeUpdateCommandBuilder : CommandBuilder<FakeUpdateCommand>
    {
        public FakeUpdateCommandBuilder(IFixture fixture, CommandSeed<FakeUpdateCommand> seed)
            : base(fixture, seed)
        {
        }

        protected override void Setup(FakeUpdateCommand command)
        {
            command.Data = Fixture.Create<string>();
        }
    }
}