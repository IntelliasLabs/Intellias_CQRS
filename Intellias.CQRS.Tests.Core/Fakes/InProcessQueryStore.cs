using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// InProcessQueryStore
    /// </summary>
    public class InProcessQueryStore<TQueryModel> : IQueryModelReader<TQueryModel>,
        IQueryModelWriter<TQueryModel>
        where TQueryModel: class, IQueryModel, new()
    {
        private readonly Dictionary<string, object> store;

        /// <summary>
        /// InProcessQueryStore
        /// </summary>
        public InProcessQueryStore(Dictionary<Type, Dictionary<string, object>> tables)
        {
            var has = tables.ContainsKey(typeof(TQueryModel));
            if(has)
            {
                store = tables[typeof(TQueryModel)];
            }
            else
            {
                store = new Dictionary<string, object>();
                tables.Add(typeof(TQueryModel), store);
            }
        }

        /// <summary>
        /// Deletes all TQueryModels
        /// </summary>
        /// <returns></returns>
        public Task ClearAsync()
        {
            store.Clear();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes root TQueryModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id)
        {
            store.Remove(id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets TQueryModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TQueryModel> GetAsync(string id)
        {
            var model = store[id];
            return Task.FromResult((TQueryModel)model);
        }

        /// <summary>
        /// Gets all TQueryModels
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<TQueryModel>> GetAllAsync()
        {
            return await Task.FromResult(store.Values.Cast<TQueryModel>().ToList().AsReadOnly());
        }

        /// <inheritdoc />
        public Task UpdateAsync(string id, Action<TQueryModel> model)
        {
            var queryModel = store[id];
            model?.Invoke((TQueryModel)queryModel);
            return Task.FromResult(queryModel);
        }

        /// <summary>
        /// Creates TQueryModel
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public Task CreateAsync(TQueryModel queryModel)
        {
            store.Add(queryModel.Id, queryModel);
            return Task.FromResult(queryModel);
        }
    }
}
