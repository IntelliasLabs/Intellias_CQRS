using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;
using Microsoft.WindowsAzure.Storage;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Mock for Immutable query model storage.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of immutable uery model.</typeparam>
    public class InProcessImmutableQueryModelStoragev2<TQueryModel> :
        CQRS.Core.Queries.Immutable.Interfaces.IImmutableQueryModelReader<TQueryModel>,
        CQRS.Core.Queries.Immutable.Interfaces.IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        private readonly InProcessTableStorage<TQueryModel> storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessImmutableQueryModelStoragev2{TQueryModel}"/> class.
        /// </summary>
        /// <param name="storage">Storage in memory.</param>
        public InProcessImmutableQueryModelStoragev2(InProcessTableStorage<TQueryModel> storage)
        {
            this.storage = storage;
        }

        /// <inheritdoc />
        public Task<TQueryModel> FindAsync(string id, int version)
        {
            var queryModel = storage.FirstOrDefault(q => q.Id == id && q.Version == version);
            return Task.FromResult(queryModel);
        }

        /// <inheritdoc />
        public Task<TQueryModel> FindLatestAsync(string id)
        {
            var queryModel = storage.Where(qm => qm.Id == id).OrderByDescending(qm => qm.Version).FirstOrDefault();
            return Task.FromResult(queryModel);
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetAsync(string id, int version)
        {
            var result = await FindAsync(id, version);
            return result ?? throw new KeyNotFoundException();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> GetLatestAsync(string id)
        {
            var queryModel = await FindLatestAsync(id);
            return queryModel ?? throw new KeyNotFoundException();
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Collection of entities.</returns>
        public IReadOnlyCollection<TQueryModel> GetAll()
        {
            return storage;
        }
    }
}