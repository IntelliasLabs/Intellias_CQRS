using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intellias.CQRS.Core.Messages
{
    /// <inheritdoc />
    public abstract class AbstractMessage : IMessage
    {
        /// <summary>
        /// Constructs abstract message
        /// </summary>
        protected AbstractMessage()
        {
            TypeName = GetType().AssemblyQualifiedName;
        }

        /// <inheritdoc />
        [Key]
        [DataType(DataType.Text)]
        [JsonProperty("id")]
        public string Id { get; set; } = Unified.NewCode();

        /// <inheritdoc />
        [DataType(DataType.Text)]
        public string AggregateRootId { get; set; } = string.Empty;

        /// <inheritdoc />
        [DataType(DataType.Text)]
        public string CorrelationId { get; set; } = string.Empty;

        /// <inheritdoc />
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; } = DateTime.UtcNow;

        /// <inheritdoc />
        [Required]
        public string TypeName { get; private set; }

        /// <inheritdoc />
        public IDictionary<MetadataKey, string> Metadata { get; } = new Dictionary<MetadataKey, string>();

        /// <inheritdoc />
        public override string ToString() => Id;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is AbstractMessage a)
            {
                return Equals(a);
            }

            return false;
        }

        private static bool IsTransient(AbstractMessage obj)
        {
            return obj != null && string.IsNullOrWhiteSpace(obj.Id);
        }

        /// <summary>
        /// Base equality operator
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Is equal</returns>
        protected virtual bool Equals(AbstractMessage other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Id.Equals(other.Id, StringComparison.InvariantCultureIgnoreCase))
            {
                var otherType = other.GetType();
                var thisType = GetType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Unified.Decode(Id).GetHashCode();
        }

        /// <summary>
        /// Parse abstract message
        /// </summary>
        /// <param name="json">input json</param>
        /// <returns>object</returns>
        public static IMessage ParseJson(string json)
        {
            var jObject = JObject.Parse(json);
            string typeName;
            typeName = jObject.SelectToken(nameof(typeName)).ToString();
            return (IMessage)jObject.ToObject(Type.GetType(typeName));
        }

        /// <summary>
        /// Copy metadata to another message instance
        /// </summary>
        /// <param name="to">target object</param>
        public void CopyMetadata(AbstractMessage to)
        {
            foreach (var key in this.Metadata.Keys)
            {
                to.Metadata[key] = this.Metadata[key];
            }
        }

        /// <summary>
        /// Converts abstract message to another type
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public TMessage ToType<TMessage>()
            where TMessage : AbstractMessage, new()
        {
            var result = new TMessage
            {
                Id = Unified.NewCode(),
                AggregateRootId = this.AggregateRootId,
                CorrelationId = this.CorrelationId
            };

            this.CopyMetadata(result);

            return result;
        }
    }
}
