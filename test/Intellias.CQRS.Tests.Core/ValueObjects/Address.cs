using System;
using Intellias.CQRS.Core.Domain;

namespace Intellias.CQRS.Tests.Core.ValueObjects
{
    /// <summary>
    /// Address value object.
    /// </summary>
    public class Address : ValueObject<Address>
    {
        /// <summary>
        /// Address Line.
        /// </summary>
        public string AddressLine { get; set; } = string.Empty;

        /// <summary>
        /// Post Code.
        /// </summary>
        public string PostCode { get; set; } = string.Empty;

        /// <inheritdoc />
        protected override bool EqualsCore(Address other) => string.Equals(AddressLine, other.AddressLine, StringComparison.InvariantCulture);

        /// <inheritdoc />
        protected override int GetHashCodeCore()
        {
            return AddressLine.GetHashCode();
        }
    }
}
