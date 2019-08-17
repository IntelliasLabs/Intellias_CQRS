namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Base type for value-objects.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    public abstract class ValueObject<T>
        where T : ValueObject<T>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is T valueObject && EqualsCore(valueObject);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeCore();
        }

        /// <summary>
        /// Base equality method.
        /// </summary>
        /// <param name="other">Other entity.</param>
        /// <returns>Is equal.</returns>
        protected abstract bool EqualsCore(T other);

        /// <summary>
        /// Hash code code generator.
        /// </summary>
        /// <returns>Hash id.</returns>
        protected abstract int GetHashCodeCore();
    }
}
