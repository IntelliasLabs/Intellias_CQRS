using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
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
        private readonly Dictionary<KeyValuePair<string, string>, TQueryModel> store;

        /// <summary>
        /// 
        /// </summary>
        public InProcessQueryStore()
        {
            store = new Dictionary<KeyValuePair<string, string>, TQueryModel>();
        }

        /// <summary>
        /// Deletes all TQueryModels
        /// </summary>
        /// <returns></returns>
        public Task DeleteAllAsync()
        {
            store.Clear();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteAsync(string parentId, string id)
        {
            store.Remove(new KeyValuePair<string, string>(parentId, id));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes root TQueryModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id)
        {
            store.Remove(new KeyValuePair<string, string>(Unified.Dummy, id));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets TQueryModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TQueryModel> GetAsync(string id)
        {
            var model = store[new KeyValuePair<string, string>(Unified.Dummy, id)];
            return Task.FromResult(model);
        }

        /// <inheritdoc />
        public Task<TQueryModel> GetAsync(string parentId, string id)
        {
            var model = store[new KeyValuePair<string, string>(parentId, id)];
            return Task.FromResult(model);
        }

        /// <summary>
        /// Gets all TQueryModels
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

        /// <inheritdoc />
        public Task<CollectionQueryModel<TQueryModel>> GetAllAsync(string parentId)
        {
            var qmList = store
                .Where(kvp => kvp.Key.Key == parentId)
                .Select(kvp => kvp.Value)
                .ToList();

            return Task.FromResult(new CollectionQueryModel<TQueryModel>
            {
                Items = qmList,
                Total = qmList.Count
            });
        }

        /// <summary>
        /// Updates TQueryModelby Id
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<TQueryModel> UpdateAsync(TQueryModel newQueryModel)
        {
            store[new KeyValuePair<string, string>(newQueryModel.ParentId, newQueryModel.Id)] = newQueryModel;
            return Task.FromResult(newQueryModel);
        }

        /// <summary>
        /// Updates collection of TQueryModel by Ids
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionQueryModel<TQueryModel>> UpdateAllAsync(IEnumerable<TQueryModel> newCollection)
        {
            var queryModels = newCollection.ToList();
            queryModels.ForEach(item => 
                store[new KeyValuePair<string, string>(item.ParentId, item.Id)] = item);

            return Task.FromResult(new CollectionQueryModel<TQueryModel>
            {
                Items = queryModels,
                Total = queryModels.Count
            });
        }

        /// <summary>
        /// Creates TQueryModel
        /// </summary>
        /// <param name="newQueryModel"></param>
        /// <returns></returns>
        public Task<TQueryModel> CreateAsync(TQueryModel newQueryModel)
        {
            store.Add(new KeyValuePair<string, string>(newQueryModel.ParentId, newQueryModel.Id), newQueryModel);
            return Task.FromResult(newQueryModel);
        }

        /// <summary>
        /// Creates collection of TQueryModels
        /// </summary>
        /// <param name="newCollection"></param>
        /// <returns></returns>
        public Task<CollectionQueryModel<TQueryModel>> CreateAllAsync(IEnumerable<TQueryModel> newCollection)
        {
            var queryModels = newCollection.ToList();
            queryModels.ForEach(item => store.Add(new KeyValuePair<string, string>(item.ParentId, item.Id), item));

            return Task.FromResult(new CollectionQueryModel<TQueryModel>
            {
                Items = queryModels,
                Total = queryModels.Count
            });
        }
    }
}
