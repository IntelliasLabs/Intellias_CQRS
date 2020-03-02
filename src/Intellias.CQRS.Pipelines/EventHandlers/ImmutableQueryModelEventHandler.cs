using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using MediatR;

namespace Intellias.CQRS.Pipelines.EventHandlers
{
    /// <summary>
    /// Event handler for <see cref="IImmutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Query model type.</typeparam>
    public abstract class ImmutableQueryModelEventHandler<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableQueryModelEventHandler{TQueryModel}"/> class.
        /// </summary>
        /// <param name="reader">Value for <see cref="Reader"/>.</param>
        /// <param name="writer">Value for <see cref="Writer"/>.</param>
        /// <param name="mediator">Value for <see cref="Mediator"/>.</param>
        protected ImmutableQueryModelEventHandler(
            IImmutableQueryModelReader<TQueryModel> reader,
            IImmutableQueryModelWriter<TQueryModel> writer,
            IMediator mediator)
        {
            Reader = reader;
            Writer = writer;
            Mediator = mediator;
        }

        /// <summary>
        /// <typeparamref name="TQueryModel"/> storage reader.
        /// </summary>
        protected IImmutableQueryModelReader<TQueryModel> Reader { get; }

        /// <summary>
        /// <typeparamref name="TQueryModel"/> storage writer.
        /// </summary>
        protected IImmutableQueryModelWriter<TQueryModel> Writer { get; }

        /// <summary>
        /// Instance of MediatR.
        /// </summary>
        protected IMediator Mediator { get; }

        /// <summary>
        /// Should be true if <typeparamref name="TQueryModel"/> is private and changes in it shouldn't publish signals.
        /// </summary>
        protected virtual bool IsPrivateQueryModel { get; } = false;

        /// <summary>
        /// Updates query model.
        /// </summary>
        /// <param name="notification">Event notification.</param>
        /// <param name="getSnapshotId">Returns snapshot id from provided event.</param>
        /// <param name="setup">Update query model.</param>
        /// <typeparam name="TEvent">Event type.</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task HandleAsync<TEvent>(
            IntegrationEventNotification<TEvent> notification,
            Func<TEvent, SnapshotId> getSnapshotId,
            Action<TEvent, TQueryModel> setup)
            where TEvent : IIntegrationEvent
        {
            var @event = notification.IntegrationEvent;
            var snapshotId = getSnapshotId(@event);

            // Find query model.
            var model = await Reader.FindLatestAsync(snapshotId.EntryId) ?? new TQueryModel();
            if (model.AppliedEvent.Created >= @event.Created)
            {
                await Mediator.Publish(new EventAlreadyAppliedNotification(@event, model));
                return;
            }

            // Update query model.
            model.Id = snapshotId.EntryId;
            model.Version = snapshotId.EntryVersion;
            model.AppliedEvent = new AppliedEvent { Id = @event.Id, Created = @event.Created };

            setup(@event, model);

            // Save updated query model.
            var created = await Writer.CreateAsync(model);
            var signal = QueryModelChangedSignal.CreateFromSource(@event, created.Id, created.Version, created.GetType(), QueryModelChangeOperation.Create);

            await Mediator.Publish(new QueryModelChangedNotification(signal)
            {
                IsReplay = @event.IsReplay,
                IsPrivate = IsPrivateQueryModel
            });
        }
    }
}