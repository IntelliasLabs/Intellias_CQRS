﻿using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    /// <summary>
    /// Publishing events to Azure Service Bus
    /// </summary>
    public class AzureServiceTopicEventBus : IEventBus
    {

        #region Private members

        
        private readonly ITopicClient topicClient;


        #endregion

        #region Constructors

        
        /// <summary>
        /// Creates an instance of EventBus
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topic"></param>
        public AzureServiceTopicEventBus(string connectionString, string topic)
        {
            topicClient = new TopicClient(connectionString, topic);
        }


        #endregion

        #region Implementations

        
        /// <inheritdoc />
        public async Task PublishAsync<T>(T @event) where T : IEvent
        {
            await topicClient.SendAsync(@event.ToBusMessage());
        }


        #endregion

    }
}
