using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Mutable;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessMutableQueryModelStorage<TQueryModel> :
        IMutableQueryModelReader<TQueryModel>,
        IMutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IMutableQueryModel, new()
    {
        private readonly InProcessTableStorage<TQueryModel> storage;

        public InProcessMutableQueryModelStorage(InProcessTableStorage<TQueryModel> storage)
        {
            this.storage = storage;
        }

        public Task<TQueryModel> FindAsync(string id)
        {
            var queryModel = storage.FirstOrDefault(q => q.Id == id);
            return Task.FromResult(queryModel);
        }

        public async Task<TQueryModel> GetAsync(string id)
        {
            var result = await FindAsync(id);
            return result ?? throw new KeyNotFoundException();
        }

        public Task<IReadOnlyCollection<TQueryModel>> GetAllAsync()
        {
            return Task.FromResult((IReadOnlyCollection<TQueryModel>)storage);
        }

        public Task<IReadOnlyCollection<TQueryModel>> GetAllAsync(IReadOnlyCollection<string> ids)
        {
            IReadOnlyCollection<TQueryModel> queryModels = storage.Where(s => ids.Contains(s.Id)).ToArray();
            return Task.FromResult(queryModels);
        }

        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var existing = await FindAsync(model.Id);
            if (existing != null)
            {
                throw new StorageException();
            }

            storage.Add(model);

            model.Timestamp = DateTimeOffset.UtcNow;
            model.ETag = Unified.NewCode();

            return model;
        }

        public async Task<TQueryModel> ReplaceAsync(TQueryModel model)
        {
            var existing = await FindAsync(model.Id);
            if (existing == null)
            {
                throw new StorageException();
            }

            storage.RemoveAll(m => m.Id == existing.Id);
            storage.Add(model);

            model.Timestamp = DateTimeOffset.UtcNow;
            model.ETag = Unified.NewCode();

            return model;
        }
    }
}