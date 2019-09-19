using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessImmutableQueryModelStorage<TQueryModel> :
        IImmutableQueryModelReader<TQueryModel>,
        IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        private readonly InProcessTableStorage<TQueryModel> storage;

        public InProcessImmutableQueryModelStorage(InProcessTableStorage<TQueryModel> storage)
        {
            this.storage = storage;
        }

        public Task<TQueryModel?> FindAsync(string id, int version)
        {
            var queryModel = storage.FirstOrDefault(q => q.Id == id && q.Version == version);
            return Task.FromResult(queryModel);
        }

        public async Task<TQueryModel> GetAsync(string id, int version)
        {
            var result = await FindAsync(id, version);
            return result ?? throw new KeyNotFoundException();
        }

        public Task<TQueryModel?> GetLatestAsync(string id)
        {
            var queryModel = storage.Where(qm => qm.Id == id).OrderByDescending(qm => qm.Version).FirstOrDefault();
            return Task.FromResult(queryModel);
        }

        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var existing = await FindAsync(model.Id, model.Version);
            if (existing != null)
            {
                throw new StorageException();
            }

            storage.Add(model);

            model.Timestamp = DateTimeOffset.UtcNow;

            return model;
        }

        public IReadOnlyCollection<TQueryModel> GetAll()
        {
            return storage;
        }
    }
}