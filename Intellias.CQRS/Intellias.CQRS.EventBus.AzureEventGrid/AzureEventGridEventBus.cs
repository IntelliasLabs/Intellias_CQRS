using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventBus.AzureEventGrid.Extensions;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace Intellias.CQRS.EventBus.AzureEventGrid
{
    /// <summary>
    /// AzureEventGrid CommandBus
    /// </summary>
    public class AzureEventGridEventBus : IEventBus, IDisposable
    {
        private readonly IEventGridClient client;
        private readonly Uri topicHostname;

        private bool disposed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="key"></param>
        public AzureEventGridEventBus(string host, string key)
        {
            topicHostname = new Uri(host);

            var topicCredentials = new TopicCredentials(key);
            client = new EventGridClient(topicCredentials);
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            var @event = new List<EventGridEvent> { msg.ToEventGridEvent() };

            await client
                .PublishEventsAsync(topicHostname.Host, @event)
                .ConfigureAwait(false);
            return await Task.FromResult(ExecutionResult.Success);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~AzureEventGridEventBus()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose(bool)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            { return; }

            if (disposing)
            {
                // Освобождаем управляемые ресурсы
                client?.Dispose();
            }

            // освобождаем неуправляемые объекты
            disposed = true;

            /* Обращение к методу Dispose базового класса
             No base class implementing IDisposable,
             Uncomment if present.
            
             base.Dispose(disposing);
            */
        }
    }
}
