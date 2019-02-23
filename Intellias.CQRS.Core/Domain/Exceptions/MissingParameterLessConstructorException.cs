using System;

namespace Intellias.CQRS.Core.Domain.Exceptions
{
    /// <inheritdoc />
    public class MissingParameterLessConstructorException : Exception
    {
        /// <inheritdoc />
        public MissingParameterLessConstructorException(Type type)
            : base($"{type.FullName} has no constructor without paramerters. This can be either public or private")
        { }

        /// <inheritdoc />
        public MissingParameterLessConstructorException()
        {
        }

        /// <inheritdoc />
        public MissingParameterLessConstructorException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public MissingParameterLessConstructorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}