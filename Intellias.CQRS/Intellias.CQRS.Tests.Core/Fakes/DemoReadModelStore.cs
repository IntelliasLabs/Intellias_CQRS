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
    public class DemoReadModelStore : IQueryModelStore<DemoQueryModel>
    {
        private readonly Dictionary<string, DemoQueryModel> store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DemoReadModelStore(Dictionary<string, DemoQueryModel> store)
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
        public Task<DemoQueryModel> GetAsync(string id)
        {
            var model = store[id];
            return Task.FromResult(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<CollectionQueryModel<DemoQueryModel>> GetAllAsync()
        {
            return Task.FromResult(new CollectionQueryModel<DemoQueryModel>
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
        public Task<DemoQueryModel> UpdateAsync(DemoQueryModel newQueryModel)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionQueryModel<DemoQueryModel>> UpdateAllAsync(IEnumerable<DemoQueryModel> newCollection)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<DemoQueryModel> CreateAsync(DemoQueryModel newQueryModel)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionQueryModel<DemoQueryModel>> CreateAllAsync(IEnumerable<DemoQueryModel> newCollection)
        {
            throw new System.NotImplementedException();
        }
    }
}
