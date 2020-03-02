using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Pipelines.EventHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using Intellias.CQRS.Tests.Core.Fakes;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using MediatR;

namespace Intellias.CQRS.Tests.Core.EventHandlers
{
    /// <summary>
    /// Core event handler on mutable query models for test purposes.
    /// </summary>
    /// <typeparam name="TEventHandler">Event handler type.</typeparam>
    /// <typeparam name="TQueryModel">Query model type.</typeparam>
    public abstract class MutableQueryModelEventHandlerTests<TEventHandler, TQueryModel>
        where TQueryModel : class, IMutableQueryModel, new()
        where TEventHandler : MutableQueryModelEventHandler<TQueryModel>
    {
        /// <summary>
        /// Fixture.
        /// </summary>
        protected abstract Fixture Fixture { get; }

        /// <summary>
        /// Storage.
        /// </summary>
        protected abstract InProcessMutableQueryModelStorage<TQueryModel> Storage { get; }

        /// <summary>
        /// Event handler.
        /// </summary>
        protected abstract TEventHandler EventHandler { get; }

        /// <summary>
        /// ctor.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Type of integration event.</typeparam>
        /// <param name="getId">Func which is gets Id.</param>
        /// <param name="getExpectedQueryModel">Func that gets expected query model.</param>
        /// <returns>Simple Task.</returns>
        protected async Task TestHandleAsync<TIntegrationEvent>(
            Func<TIntegrationEvent, string> getId,
            Func<TIntegrationEvent, Task<TQueryModel>> getExpectedQueryModel)
            where TIntegrationEvent : IIntegrationEvent
        {
            var request = Fixture.Create<IntegrationEventNotification<TIntegrationEvent>>();
            var @event = request.IntegrationEvent;

            var id = getId(@event);
            var expectedQueryModel = await getExpectedQueryModel(@event);

            await HandleGenericEventAsync(EventHandler, @event);

            var queryMode = await Storage.GetAsync(id);

            queryMode.Should().BeEquivalentTo(expectedQueryModel, options => options.ForMutableQueryModel());
        }

        /// <summary>
        /// Used for setting up query model before verification it.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Type of integration event.</typeparam>
        /// <param name="event">Event.</param>
        /// <param name="getId">Func that should return Id.</param>
        /// <param name="setup">Setup func for query model.</param>
        /// <returns>Set up query model.</returns>
        protected async Task<TQueryModel> SetupQueryModelAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            Func<TIntegrationEvent, string> getId,
            Action<TQueryModel> setup = null)
            where TIntegrationEvent : Event
        {
            var id = getId(@event);

            // Setup query model in store.
            var queryModel = Fixture.Build<TQueryModel>()
                .With(s => s.Id, id)
                .With(s => s.AppliedEvent, new AppliedEvent { Id = Unified.NewCode(), Created = @event.Created.AddMinutes(-1) })
                .Create();

            setup?.Invoke(queryModel);

            await Storage.CreateAsync(queryModel);

            queryModel.AppliedEvent = new AppliedEvent { Id = @event.Id, Created = @event.Created };

            return queryModel;
        }

        private async Task HandleGenericEventAsync<TIntegrationEvent>(TEventHandler handler, TIntegrationEvent @event)
        {
            var eventType = @event.GetType();
            var eventRequestType = typeof(IntegrationEventNotification<>).MakeGenericType(eventType);
            var eventRequest = Activator.CreateInstance(eventRequestType, @event);

            var methodInfo = EventHandler.GetType().GetMethod(nameof(INotificationHandler<IntegrationEventNotification<IntegrationEvent>>.Handle), new[] { eventRequestType, typeof(CancellationToken) });
            if (methodInfo == null)
            {
                throw new InvalidOperationException($"No handler is found for event of type '{eventType}'.");
            }

            await (Task)methodInfo.Invoke(handler, new[] { eventRequest, CancellationToken.None });
        }
    }
}
