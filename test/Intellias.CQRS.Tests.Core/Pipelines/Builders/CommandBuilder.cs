using System;
using AutoFixture;
using AutoFixture.Kernel;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Infrastructure.Builders;

namespace Intellias.CQRS.Tests.Core.Pipelines.Builders
{
    public abstract class CommandBuilder<TCommand> : SeededBuilderBase<TCommand, CommandSeed<TCommand>>
        where TCommand : Command, new()
    {
        protected CommandBuilder(IFixture fixture, CommandSeed<TCommand> seed)
            : base(fixture, seed)
        {
        }

        protected abstract void Setup(TCommand command);

        protected override TCommand Create(ISpecimenContext context)
        {
            var userId = (Seed.UserId ?? Guid.NewGuid()).ToString();
            var command = new TCommand
            {
                Id = Unified.NewCode(),
                AggregateRootId = Seed.AggregateRootId ?? Unified.NewCode(),
                CorrelationId = Unified.NewCode(),
                ExpectedVersion = 0,
                Metadata =
                {
                    [MetadataKey.UserId] = userId,
                    [MetadataKey.Roles] = "[]"
                },
                Actor =
                {
                    IdentityId = userId,
                    UserId = userId
                },
                Principal =
                {
                    IdentityId = userId,
                    UserId = userId
                }
            };

            Setup(command);

            Seed.Setup?.Invoke(command);

            return command;
        }
    }
}