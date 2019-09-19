using System.Text;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Commands;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.CommandBus.AzureServiceBus.Extensions
{
    internal static class ServiceBusMessageExtensions
    {
        public static Message ToBusMessage(this ICommand command) =>
            new Message(Encoding.UTF8.GetBytes(command.ToJson()))
            {
                MessageId = command.Id,
                ContentType = command.GetType().FullName,
                PartitionKey = command.AggregateRootId,
                CorrelationId = command.CorrelationId,
                SessionId = command.AggregateRootId
            };
    }
}
