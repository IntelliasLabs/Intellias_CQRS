using System;
using Intellias.CQRS.Core.Domain;

namespace Intellias.CQRS.Tests.Core.ValueObjects
{
    /// <summary>
    /// Address value object
    /// </summary>
    public class Address : ValueObject<Address>
    {
        /// <summary>
        /// Address Line
        /// </summary>
        public string AddressLine { set; get; } = string.Empty;

        /// <summary>
        /// Post Code
        /// </summary>
        public string PostCode { set; get; }

        /// <inheritdoc />
        protected override bool EqualsCore(Address other) => string.Equals(this.AddressLine, other.AddressLine, StringComparison.InvariantCulture);

        /// <inheritdoc />
        protected override int GetHashCodeCore()
        {
            return AddressLine.GetHashCode();
        }
    }
}
