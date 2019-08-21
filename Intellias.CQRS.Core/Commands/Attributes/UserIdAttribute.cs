using System;

namespace Intellias.CQRS.Core.Commands.Attributes
{
    /// <summary>
    /// Custom metadata marker that user id should be verified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UserIdAttribute : Attribute
    {
    }
}
