namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var valueObject = obj as T;
            return !ReferenceEquals(valueObject, null) && EqualsCore(valueObject);
        }

        /// <inheritdoc />
        protected abstract bool EqualsCore(T other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        /// <inheritdoc />
        protected abstract int GetHashCodeCore();

        /// <inheritdoc />
        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <inheritdoc />
        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            return !(a == b);
        }
    }
}
