using System;
using System.Collections.Generic;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Singleton backing collection for tests
    /// </summary>
    public static class InProcessQueryStoreInstance
    {
        /// <summary>
        /// Tables
        /// </summary>
        public static Dictionary<Type, Dictionary<string, object>> Tables { get; } = new Dictionary<Type, Dictionary<string, object>>();
    }
}
