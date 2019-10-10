using System.Diagnostics.CodeAnalysis;

namespace Intellias.CQRS.Core.Results.Errors
{
    /// <summary>
    /// Info about Error Code.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ErrorCodeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCodeInfo"/> class.
        /// </summary>
        /// <param name="subDomain">SubDomain as prefix.</param>
        /// <param name="internalCode">InternalCode.</param>
        /// <param name="message">Message.</param>
        public ErrorCodeInfo(string subDomain, string internalCode, string message)
        {
            Code = $"{subDomain}.{internalCode}";
            Message = message;
        }

        /// <summary>
        /// Code.
        /// </summary>
        public string Code { get; protected set; }

        /// <summary>
        /// Message.
        /// </summary>
        public string Message { get; protected set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Message;
        }
    }
}
