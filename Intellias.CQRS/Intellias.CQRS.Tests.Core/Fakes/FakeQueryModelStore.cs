using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// 
    /// </summary>
    public class FakeQueryModelStore<TQueryModel> : IQueryModelStore<TQueryModel>
        where TQueryModel: class, IQueryModel
    {
        private readonly Dictionary<string, TQueryModel> store;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public FakeQueryModelStore(Dictionary<string, TQueryModel> store)
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
        public Task<TQueryModel> GetAsync(string id)
        {
            var model = store[id];
            return Task.FromResult(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<CollectionQueryModel<TQueryModel>> GetAllAsync()
        {
            return Task.FromResult(new CollectionQueryModel<TQueryModel>
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
        public Task<TQueryModel> UpdateAsync(TQueryModel newQueryModel)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionQueryModel<TQueryModel>> UpdateAllAsync(IEnumerable<TQueryModel> newCollection)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<TQueryModel> CreateAsync(TQueryModel newQueryModel)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NOT IMPOLEMENTED
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionQueryModel<TQueryModel>> CreateAllAsync(IEnumerable<TQueryModel> newCollection)
        {
            throw new System.NotImplementedException();
        }
    }
}
