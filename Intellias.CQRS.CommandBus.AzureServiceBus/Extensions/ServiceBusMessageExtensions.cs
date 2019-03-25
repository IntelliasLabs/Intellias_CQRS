﻿using System.Text;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Config;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Intellias.CQRS.CommandBus.AzureServiceBus.Extensions
{
    internal static class ServiceBusMessageExtensions
    {
        public static Message ToBusMessage(this ICommand command) =>
            new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command, CqrsSettings.JsonConfig())))
            {
                MessageId = command.Id,
                ContentType = command.GetType().FullName,
                PartitionKey = command.AggregateRootId,
                CorrelationId = command.CorrelationId
            };
    }
}