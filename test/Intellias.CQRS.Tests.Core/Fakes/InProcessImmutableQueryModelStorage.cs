using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Queries.Immutable;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Mock for Immutable query model storage.
    /// </summary>
    /// <typeparam name="TQueryModel">Type of immutable uery model.</typeparam>
    public class InProcessImmutableQueryModelStorage<TQueryModel> :
        IImmutableQueryModelReader<TQueryModel>,
        IImmutableQueryModelWriter<TQueryModel>
        where TQueryModel : class, IImmutableQueryModel, new()
    {
        private readonly InProcessTableStorage<TQueryModel> storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessImmutableQueryModelStorage{TQueryModel}"/> class.
        /// </summary>
        /// <param name="storage">Storage in memory.</param>
        public InProcessImmutableQueryModelStorage(InProcessTableStorage<TQueryModel> storage)
        {
            this.storage = storage;
        }

        /// <inheritdoc />
        public Task<TQueryModel> FindAsync(string id, int version)
        {
            var result = storage.FirstOrDefault(q => q.Id == id && q.Version == version);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<TQueryModel> FindLatestAsync(string id)
        {
            var result = storage.Where(qm => qm.Id == id).OrderByDescending(qm => qm.Version).FirstOrDefault();
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task<TQueryModel> FindEqualOrLessAsync(string id, int fromVersion)
        {
            var result = storage.Where(qm => qm.Id == id && qm.Version <= fromVersion).OrderByDescending(qm => qm.Version).FirstOrDefault();
            return Task.FromResult(result);
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
            var result = await FindLatestAsync(id);
            return result ?? throw new KeyNotFoundException();
        }

        /// <inheritdoc />
        public async Task<TQueryModel> CreateAsync(TQueryModel model)
        {
            var existingModel = await FindAsync(model.Id, model.Version);
            if (existingModel != null)
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

        /// <inheritdoc />
        public Task<TQueryModel> GetEqualOrLessAsync(string id, int fromVersion)
        {
            var model = FindEqualOrLessAsync(id, fromVersion);
            return model ?? throw new KeyNotFoundException();
        }
    }
}