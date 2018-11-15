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
    public class InProcessQueryStore<TQueryModel> : IQueryModelStore<TQueryModel>
        where TQueryModel: class, IQueryModel
    {
        private readonly Dictionary<string, TQueryModel> store;

        /// <summary>
        /// 
        /// </summary>
        public InProcessQueryStore()
        {
            store = new Dictionary<string, TQueryModel>();
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
        /// DeleteAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id)
        {
            store.Remove(id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// GetAsync
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
        /// UpdateAsync
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<TQueryModel> UpdateAsync(TQueryModel newQueryModel)
        {
            store[newQueryModel.Id] = newQueryModel;
            return Task.FromResult(newQueryModel);
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
        /// CreateAsync
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<TQueryModel> CreateAsync(TQueryModel newQueryModel)
        {
            store.Add(newQueryModel.Id, newQueryModel);
            return Task.FromResult(newQueryModel);
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
