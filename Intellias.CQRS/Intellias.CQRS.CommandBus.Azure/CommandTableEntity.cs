using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Intellias.CQRS.CommandBus.Azure
{
    /// <summary>
    /// Command Table Entity
    /// </summary>
    public class CommandTableEntity : TableEntity, ICommand
    {
        private readonly ICommand cmd;

        /// <inheritdoc />
        public CommandTableEntity(ICommand cmd, string content)
        {
            this.cmd = cmd;
            PartitionKey = cmd.AggregateRootId;
            RowKey = $"{cmd.ExpectedVersion}.{cmd.Id}";
            Data = content;
            ETag = "*";
        }

        /// <summary>
        /// Holds the entity data
        /// </summary>
        public string Data { set; get; }

        /// <inheritdoc />
        public string Id => cmd.Id;

        /// <inheritdoc />
        public int ExpectedVersion { get => cmd.ExpectedVersion; set => throw new NotSupportedException(); }

        /// <inheritdoc />
        public string AggregateRootId { get => cmd.AggregateRootId; set => throw new NotSupportedException(); }

        /// <inheritdoc />
        public string UserId { get => cmd.UserId; set => throw new NotSupportedException(); }

        /// <inheritdoc />
        public DateTime Created => cmd.Created;

        /// <inheritdoc />
        public IDictionary<MetadataKey, string> Metadata => cmd.Metadata;

        /// <summary>
        /// Returns deserialized data object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetValue<T>() =>
            string.IsNullOrEmpty(Data)
                ? default(T)
                : JsonConvert.DeserializeObject<T>(Data);
    }
}
