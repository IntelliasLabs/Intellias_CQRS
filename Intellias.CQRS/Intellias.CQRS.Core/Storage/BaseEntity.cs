using Intellias.CQRS.Core.Messages;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// Base storage entity
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity unified code identifier
        /// </summary>
        [Key]
        [DataType(DataType.Text)]
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Partition Key
        /// </summary>
        [DataType(DataType.Text)]
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// DateTime of creation
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        [JsonProperty("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// DateTime of modification
        /// </summary>
        [DataType(DataType.DateTime)]
        [JsonProperty("modified")]
        public DateTime Modified { get; set; } = DateTime.UtcNow;

        /// <inheritdoc />
        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        /// <inheritdoc />
        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && string.IsNullOrWhiteSpace(obj.Id);
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        /// <inheritdoc />
        public virtual bool Equals(BaseEntity other)
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
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
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
