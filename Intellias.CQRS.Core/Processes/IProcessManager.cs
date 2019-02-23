using System.Threading.Tasks;

namespace Intellias.CQRS.Core.Processes
{
    /// <summary>
    /// IProcessManagerDataContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProcessManager<in T>
        where T : class, IProcess
    {
        /// <summary>
        /// Apply process changes
        /// </summary>
        Task ApplyAsync(T process);
    }
}
