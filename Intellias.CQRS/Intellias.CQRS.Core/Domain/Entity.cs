using System;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    public abstract class Entity : IEntity
    {
        /// <inheritdoc />
        public string Id { get; protected set; } = string.Empty;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }

            return ReferenceEquals(null, compareTo) ? false : Id.Equals(compareTo.Id, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }

            return ReferenceEquals(a, null) || ReferenceEquals(b, null) ? false : a.Equals(b);
        }

        /// <inheritdoc />
        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetType().Name + " [Id=" + Id + "]";
        }
    }
}
