using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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
            Metadata.Add(MetadataKey.TypeName, GetType().Name);
        }

        /// <inheritdoc />
        [Key]
        [DataType(DataType.Text)]
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <inheritdoc />
        [DataType(DataType.Text)]
        public string AggregateRootId { get; set; }

        /// <inheritdoc />
        [DataType(DataType.Text)]
        public string CorrelationId { get; set; }

        /// <inheritdoc />
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; } = DateTime.UtcNow;

        /// <inheritdoc />
        public IDictionary<MetadataKey, string> Metadata { get; } = new Dictionary<MetadataKey, string>();

        /// <inheritdoc />
        public override string ToString() => Id;

        /// <summary>
        /// Base equality operator
        /// </summary>
        /// <param name="x">This</param>
        /// <param name="y">Other</param>
        /// <returns>Is equal</returns>
        public static bool operator ==(AbstractMessage x, AbstractMessage y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// Base inversed equality operator
        /// </summary>
        /// <param name="x">This</param>
        /// <param name="y">Other</param>
        /// <returns>Is equal</returns>
        public static bool operator !=(AbstractMessage x, AbstractMessage y)
        {
            return !(x == y);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as AbstractMessage);
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
    }
}
