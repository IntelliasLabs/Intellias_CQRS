using System;
using AutoFixture;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Tests.Core.Infrastructure;
using Intellias.CQRS.Tests.Core.Pipelines.Builders;
using IntelliGrowth.JobProfiles.Tests.Utils.Pipelines;

namespace Intellias.CQRS.Tests.Utils
{
    public static class Fixtures
    {
        public static readonly PipelinesFixtures Pipelines = new PipelinesFixtures();

        public static TIntegrationEvent IntegrationEvent<TIntegrationEvent>(Command command, Action<TIntegrationEvent> setup)
            where TIntegrationEvent : IntegrationEvent, new()
        {
            var context = new AggregateExecutionContext(command);
            var @event = context.CreateIntegrationEvent<TIntegrationEvent>();

            setup(@event);

            return @event;
        }

        public static TCommand CommandFromBuilder<TCommand>(Func<IFixture, CommandBuilder<TCommand>> builder)
            where TCommand : Command, new()
        {
            return new Fixture()
                .Use(builder)
                .Create<TCommand>();
        }
    }
}