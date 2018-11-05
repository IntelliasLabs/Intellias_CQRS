using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.CommandBus.AzureEventGrid.Extensions;
using Intellias.CQRS.Core.Commands;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace Intellias.CQRS.CommandBus.AzureEventGrid
{
    /// <summary>
    /// AzureEventGrid CommandBus
    /// </summary>
    public class AzureEventGridCommandBus : ICommandBus, IDisposable
    {
        private readonly IEventGridClient client;
        private readonly Uri topicHostname;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="key"></param>
        public AzureEventGridCommandBus(string host, string key)
        {
            topicHostname = new Uri(host);

            var topicCredentials = new TopicCredentials(key);
            client = new EventGridClient(topicCredentials);
        }

        /// <inheritdoc />
        public async Task<ICommandResult> PublishAsync(ICommand msg)
        {
            var commands = new List<EventGridEvent> { msg.ToEventGridCommand() };

            await client
                .PublishEventsAsync(topicHostname.Host, commands)
                .ConfigureAwait(false);
            return await Task.FromResult(CommandResult.Success);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
