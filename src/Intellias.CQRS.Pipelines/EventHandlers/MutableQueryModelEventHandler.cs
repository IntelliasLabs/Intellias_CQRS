using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using MediatR;

namespace Intellias.CQRS.Pipelines.EventHandlers
{
    /// <summary>
    /// Event handler for <see cref="IMutableQueryModel"/>.
    /// </summary>
    /// <typeparam name="TQueryModel">Query model type.</typeparam>
    public abstract class MutableQueryModelEventHandler<TQueryModel>
        where TQueryModel : class, IMutableQueryModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MutableQueryModelEventHandler{TQueryModel}"/> class.
        /// </summary>
        /// <param name="reader">Value for <see cref="Reader"/>.</param>
        /// <param name="writer">Value for <see cref="Writer"/>.</param>
        /// <param name="mediator">Value for <see cref="Mediator"/>.</param>
        protected MutableQueryModelEventHandler(
            IMutableQueryModelReader<TQueryModel> reader,
            IMutableQueryModelWriter<TQueryModel> writer,
            IMediator mediator)
        {
            Reader = reader;
            Writer = writer;
            Mediator = mediator;
        }

        /// <summary>
        /// <typeparamref name="TQueryModel"/> storage reader.
        /// </summary>
        protected IMutableQueryModelReader<TQueryModel> Reader { get; }

        /// <summary>
        /// <typeparamref name="TQueryModel"/> storage writer.
        /// </summary>
        protected IMutableQueryModelWriter<TQueryModel> Writer { get; }

        /// <summary>
        /// Instance of MediatR.
        /// </summary>
        protected IMediator Mediator { get; }

        /// <summary>
        /// Updates query model.
        /// </summary>
        /// <param name="notification">Event notification.</param>
        /// <param name="getId">Returns query model id from provided event.</param>
        /// <param name="setup">Update query model.</param>
        /// <typeparam name="TEvent">Event type.</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected Task HandleAsync<TEvent>(
            IntegrationEventNotification<TEvent> notification,
            Func<TEvent, string> getId,
            Action<TEvent, TQueryModel> setup)
            where TEvent : Event
        {
            return HandleAsync(notification, getId, (e, qm) =>
            {
                setup(e, qm);
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Updates query model using async setup.
        /// </summary>
        /// <param name="notification">Event notification.</param>
        /// <param name="getId">Returns query model id from provided event.</param>
        /// <param name="setup">Update query model using async setup.</param>
        /// <typeparam name="TEvent">Event type.</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task HandleAsync<TEvent>(
            IntegrationEventNotification<TEvent> notification,
            Func<TEvent, string> getId,
            Func<TEvent, TQueryModel, Task> setup)
            where TEvent : Event
        {
            var @event = notification.IntegrationEvent;
            var id = getId(@event);

            // Find query model.
            var queryModel = await Reader.FindAsync(id) ?? new TQueryModel();
            var isNew = string.IsNullOrWhiteSpace(queryModel.AppliedEvent.Id);

            if (queryModel.AppliedEvent.Created >= @event.Created)
            {
                await Mediator.Publish(new EventAlreadyAppliedNotification(@event, queryModel));
                return;
            }

            // Update query model.
            queryModel.Id = id;
            queryModel.AppliedEvent = new AppliedEvent(@event.Id, @event.Created);

            await setup(@event, queryModel);

            // Save updated query model.
            var saved = isNew
                ? await Writer.CreateAsync(queryModel)
                : await Writer.ReplaceAsync(queryModel);

            await Mediator.Publish(new QueryModelUpdatedNotification(@event, saved));
        }
    }
}