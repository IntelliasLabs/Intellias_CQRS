using System;

namespace Intellias.CQRS.Core.Processes
{
    /// <summary>
    /// IProcessManagerDataContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProcessManagerDataContext<T> : IDisposable
        where T : class, IProcessManager
    {
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Find(string id);

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="processManager"></param>
        void Save(T processManager);
    }
}
