using System;

namespace Intellias.CQRS.Core.DataAnnotations
{
    /// <summary>
    /// Custom metadata marker that Center of Excellence should be verified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CoEIdAttribute : Attribute
    {
    }
}