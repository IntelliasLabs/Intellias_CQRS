using System;

namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// Custom metadata marker that user id should be verified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UserIdAttribute : Attribute
    {
    }
}