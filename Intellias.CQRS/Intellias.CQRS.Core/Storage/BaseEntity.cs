using Intellias.CQRS.Core.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Intellias.CQRS.Core.Storage
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity unified code identifier
        /// </summary>
        [Key]
        [DataType(DataType.Text)]
        [JsonProperty("id")]
        public virtual string Id { get; set; } = string.Empty;

        [DataType(DataType.Text)]
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [JsonProperty("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [JsonProperty("modified")]
        public DateTime Modified { get; set; } = DateTime.Now;

        [DataType(DataType.Text)]
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [DataType(DataType.Text)]
        [JsonProperty("modifiedBy")]
        public string ModifiedBy { get; set; } = string.Empty;

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }

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

        public override int GetHashCode()
        {
            return Unified.Decode(Id).GetHashCode();
        }
    }
}
