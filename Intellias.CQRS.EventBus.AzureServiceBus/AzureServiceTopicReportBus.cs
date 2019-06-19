using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class AzureServiceTopicReportBus : IReportBus
    {
        private readonly ITopicClient topicClient;

        /// <summary>
        /// Creates an instance of ReportBus
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topic"></param>
        public AzureServiceTopicReportBus(string connectionString, string topic)
        {
            topicClient = new TopicClient(connectionString, topic);
        }

        /// <inheritdoc />
        public Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessage
        {
            var busMsg = CreateBusMessage(message);
            return topicClient.SendAsync(busMsg);
        }

        private static Message CreateBusMessage(IMessage message)
        {
            return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, CqrsSettings.JsonConfig())))
            {
                MessageId = message.Id,
                ContentType = message.GetType().FullName,
                PartitionKey = message.AggregateRootId,
                CorrelationId = message.CorrelationId
            };
        }
    }
}
