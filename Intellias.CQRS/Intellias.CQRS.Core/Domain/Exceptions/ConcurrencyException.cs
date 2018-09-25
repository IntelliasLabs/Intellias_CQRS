using System;

namespace Product.Domain.Core.Domain.Exceptions
{
    /// <inheritdoc />
    public class ConcurrencyException : Exception
    {
        /// <inheritdoc />
        public ConcurrencyException(string id)
            : base($"A different version than expected was found in aggregate {id}")
        { }

        /// <inheritdoc />
        public ConcurrencyException()
        {
        }

        /// <inheritdoc />
        public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}