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
            Id = Unified.NewCode();
        }

        /// <inheritdoc />
        public string Id { get; protected set; }

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
                Equals(Id, other.Id))
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
