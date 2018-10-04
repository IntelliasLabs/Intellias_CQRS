namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Base type for value-objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var valueObject = obj as T;
            return !ReferenceEquals(valueObject, null) && EqualsCore(valueObject);
        }

        /// <summary>
        /// Base equality method
        /// </summary>
        /// <param name="other">Other entity</param>
        /// <returns>Is equal</returns>
        protected abstract bool EqualsCore(T other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        /// <summary>
        /// Hash code code generator
        /// </summary>
        /// <returns>Hash id</returns>
        protected abstract int GetHashCodeCore();

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="a">This</param>
        /// <param name="b">Other</param>
        /// <returns>Is equal</returns>
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

        /// <summary>
        /// Inversed equality
        /// </summary>
        /// <param name="a">This</param>
        /// <param name="b">Other</param>
        /// <returns>Is equal</returns>
        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            return !(a == b);
        }
    }
}
