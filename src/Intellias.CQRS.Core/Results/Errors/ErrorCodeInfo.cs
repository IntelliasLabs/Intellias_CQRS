using System;
using Intellias.CQRS.Core.Domain;
using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Results.Errors
{
    /// <summary>
    /// Info about error code.
    /// </summary>
    public class ErrorCodeInfo : ValueObject<ErrorCodeInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeInfo"/> class.
        /// </summary>
        /// <param name="code">Value for <see cref="Code"/>.</param>
        public ErrorCodeInfo(string code)
            : this(code, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeInfo"/> class.
        /// </summary>
        /// <param name="code">Value for <see cref="Code"/>.</param>
        /// <param name="message">Value for <see cref="Message"/>.</param>
        [JsonConstructor]
        public ErrorCodeInfo(string code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeInfo"/> class.
        /// </summary>
        /// <param name="subDomain">SubDomain as prefix.</param>
        /// <param name="internalCode">InternalCode.</param>
        /// <param name="message">Message.</param>
        public ErrorCodeInfo(string subDomain, string internalCode, string message)
            : this($"{subDomain}.{internalCode}", message)
        {
        }

        /// <summary>
        /// Error code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Code}: '{Message}'";
        }

        /// <inheritdoc />
        protected override bool EqualsCore(ErrorCodeInfo other)
        {
            return Code.Equals(other.Code, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        protected override int GetHashCodeCore()
        {
            return Code.GetHashCode();
        }
    }
}
