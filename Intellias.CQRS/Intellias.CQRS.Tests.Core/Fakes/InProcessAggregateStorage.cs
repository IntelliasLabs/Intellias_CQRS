using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Domain.Exceptions;
using Intellias.CQRS.Core.Storage;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public sealed class InProcessAggregateStorage<T> : IAggregateStorage<T>
        where T : IAggregateRoot, new()
    {
        private readonly Dictionary<string, Dictionary<int, IAggregateRoot>> roots = new Dictionary<string, Dictionary<int, IAggregateRoot>>();

        /// <inheritdoc />
        public void Dispose()
        {
            roots.Clear();
        }

        /// <inheritdoc />
        public Task CreateAsync(T entity)
        {
            if (!roots.ContainsKey(entity.Id))
            {
                roots.Add(entity.Id, new Dictionary<int, IAggregateRoot>());
            }

            if (!roots[entity.Id].ContainsKey(entity.Version))
            {
                roots[entity.Id].Add(entity.Version, entity);
            }
            else
            {
                throw new AggregateException();
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<T> GetAsync(string aggregateId, int version)
        {
            if (!roots.ContainsKey(aggregateId))
            {
                throw new AggregateNotFoundException(aggregateId);
            }

            if (!roots[aggregateId].ContainsKey(version))
            {
                throw new VersionNotFoundException($"Version '{version}' of aggregate '{aggregateId}' not found.");
            }

            var root = roots[aggregateId][version];

            return Task.FromResult((T)root);
        }
    }
}
