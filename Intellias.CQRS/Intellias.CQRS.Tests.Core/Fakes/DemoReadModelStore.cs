using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Storage;
using Intellias.CQRS.Tests.Core.Queries;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// 
    /// </summary>
    public class DemoReadModelStore : IReadModelStore<DemoReadModel>
    {
        private readonly Dictionary<string, DemoReadModel> store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DemoReadModelStore(Dictionary<string, DemoReadModel> store)
        {
            this.store = store;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task DeleteAllAsync()
        {
            store.Clear();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id)
        {
            store.Remove(id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ReadModelEnvelope<DemoReadModel>> GetAsync(string id)
        {
            var model = store[id];
            return Task.FromResult(new ReadModelEnvelope<DemoReadModel>(id, model, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<ReadCollectionEnvelope<DemoReadModel>> GetAllAsync()
        {
            var collection = store.Values;
            return Task.FromResult(new ReadCollectionEnvelope<DemoReadModel> (collection));
        }
    }
}
