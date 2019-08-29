using System;

namespace Intellias.CQRS.Core.Commands.Attributes
{
    /// <summary>
    /// Custom metadata marker that Center of Excelence should be verified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CoEIdAttribute : Attribute
    {
    }
}
