using AutoFixture;

namespace Intellias.CQRS.Tests.Core.Infrastructure.Builders
{
    public abstract class SeededBuilderBase<TEntity, TSeed> : BuilderBase<TEntity>
    {
        protected SeededBuilderBase(IFixture fixture, TSeed seed)
            : base(fixture)
        {
            Seed = seed;
        }

        protected TSeed Seed { get; }
    }
}