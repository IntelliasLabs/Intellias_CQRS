using System;
using System.Reflection;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Used to return assembly of handler
    /// </summary>
    public class HandlerAssemblyResolver
    {
        /// <summary>
        /// Keeps handlers assembly
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="service"></param>
        public HandlerAssemblyResolver(Func<Assembly> service)
        {
            Assembly = service?.Invoke();
        }
    }
}
