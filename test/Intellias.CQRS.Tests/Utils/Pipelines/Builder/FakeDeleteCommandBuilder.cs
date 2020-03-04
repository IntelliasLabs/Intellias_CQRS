using AutoFixture;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Builder
{
    public class FakeDeleteCommandBuilder : CommandBuilder<FakeDeleteCommand>
    {
        public FakeDeleteCommandBuilder(IFixture fixture, CommandSeed<FakeDeleteCommand> seed)
            : base(fixture, seed)
        {
        }

        protected override void Setup(FakeDeleteCommand command)
        {
        }
    }
}