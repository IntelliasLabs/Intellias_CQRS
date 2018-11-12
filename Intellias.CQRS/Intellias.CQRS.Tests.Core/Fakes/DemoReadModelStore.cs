using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
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
        public Task<DemoReadModel> GetAsync(string id)
        {
            var model = store[id];
            return Task.FromResult(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<CollectionReadModel<DemoReadModel>> GetAllAsync()
        {
            return Task.FromResult(new CollectionReadModel<DemoReadModel>
            {
                Items = store.Values.ToList(),
                Total = store.Count
            });
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<DemoReadModel> UpdateAsync(DemoReadModel newQueryModel)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionReadModel<DemoReadModel>> UpdateAllAsync(CollectionReadModel<DemoReadModel> newCollection)
        {
            throw new System.NotImplementedException();
        }
    }
}
