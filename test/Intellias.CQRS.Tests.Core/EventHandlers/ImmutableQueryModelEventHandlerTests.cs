using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using MediatR;

namespace Intellias.CQRS.Tests.Core.EventHandlers
{
    /// <summary>
    /// Core event handler on NOT mutable query models for test purposes.
    /// </summary>
    /// <typeparam name="TEventHandler">Event handler type.</typeparam>
    /// <typeparam name="TQueryModel">Query model type.</typeparam>
    public abstract class ImmutableQueryModelEventHandlerTests<TEventHandler, TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
        where TEventHandler : ImmutableQueryModelEventHandler<TQueryModel>
    {
        /// <summary>
        /// Fixture.
        /// </summary>
        protected abstract Fixture Fixture { get; }

        /// <summary>
        /// Storage.
        /// </summary>
        protected abstract InProcessImmutableQueryModelStorage<TQueryModel> Storage { get; }

        /// <summary>
        /// Event handler.
        /// </summary>
        protected abstract TEventHandler EventHandler { get; }

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Type of integration event.</typeparam>
        /// <param name="getSnapshotIdFunc">Func which is gets snapshot Id.</param>
        /// <param name="getExpectedQueryModelFunc">Func that gets expected query model.</param>
        /// <returns>Simple Task.</returns>
        protected async Task TestHandleAsync<TIntegrationEvent>(
            Func<TIntegrationEvent, SnapshotId> getSnapshotIdFunc,
            Func<TIntegrationEvent, Task<TQueryModel>> getExpectedQueryModelFunc)
            where TIntegrationEvent : Event
        {
            var eventRequest = Fixture.Create<IntegrationEventNotification<TIntegrationEvent>>();
            var @event = eventRequest.IntegrationEvent;

            var snapshotId = getSnapshotIdFunc(@event);
            var expectedQueryModel = await getExpectedQueryModelFunc(@event);

            await HandleGenericEventAsync(EventHandler, @event);

            var queryMode = await Storage.GetAsync(snapshotId.EntryId, snapshotId.EntryVersion);

            queryMode.Should().BeEquivalentTo(expectedQueryModel, options => options.ForImmutableQueryModel());
        }

        /// <summary>
        /// Used for setting up query model before verification it.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Type of integration event.</typeparam>
        /// <param name="event">Event.</param>
        /// <param name="getSnapshotIdFunc">Func that should return snapshot Id.</param>
        /// <returns>Set up query model.</returns>
        protected async Task<TQueryModel> SetupQueryModelAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            Func<TIntegrationEvent, SnapshotId> getSnapshotIdFunc)
            where TIntegrationEvent : Event
        {
            var snapshotId = getSnapshotIdFunc(@event);

            // Setup query model in store.
            var model = Fixture.Build<TQueryModel>()
                .With(s => s.Id, snapshotId.EntryId)
                .With(s => s.Version, snapshotId.EntryVersion - 1)
                .With(s => s.AppliedEvent, new AppliedEvent(Unified.NewCode(), @event.Created.AddMinutes(-1)))
                .Create();

            await Storage.CreateAsync(model);

            model.AppliedEvent = new AppliedEvent(@event.Id, @event.Created);
            model.Version = snapshotId.EntryVersion;

            return model;
        }

        private async Task HandleGenericEventAsync<TIntegrationEvent>(TEventHandler handler, TIntegrationEvent @event)
        {
            var eventType = @event.GetType();
            var requestType = typeof(IntegrationEventNotification<>).MakeGenericType(eventType);
            var eventRequest = Activator.CreateInstance(requestType, @event);

            var methodInfo = EventHandler.GetType()
                .GetMethod(nameof(INotificationHandler<IntegrationEventNotification<IntegrationEvent>>.Handle), new[] { requestType, typeof(CancellationToken) });

            if (methodInfo == null)
            {
                throw new InvalidOperationException($"No immutable query model handler is found for event of type '{eventType}'.");
            }

            await (Task)methodInfo.Invoke(handler, new[] { eventRequest, CancellationToken.None });
        }
    }
}