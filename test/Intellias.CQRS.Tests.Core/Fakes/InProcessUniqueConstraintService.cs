using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.DomainServices;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// In process implementation of <see cref="IUniqueConstraintService"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InProcessUniqueConstraintService : IUniqueConstraintService
    {
        private readonly Dictionary<string, HashSet<string>> cache = new Dictionary<string, HashSet<string>>();

        /// <inheritdoc />
        public async Task<IExecutionResult> RemoveConstraintAsync(string indexName, string value)
        {
            if (!cache.ContainsKey(indexName))
            {
                return new FailedResult($"indexName: {indexName} not registered");
            }

            if (!cache[indexName].Contains(value))
            {
                return new FailedResult($"no such value: {value} for indexName: {indexName}");
            }

            cache[indexName].Remove(value);
            return await Task.FromResult(new SuccessfulResult());
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> ReserveConstraintAsync(string indexName, string value)
        {
            if (!cache.ContainsKey(indexName))
            {
                cache.Add(indexName, new HashSet<string>());
            }

            if (cache[indexName].Contains(value))
            {
                return new FailedResult($"value: {value} for indexName: {indexName} already exists");
            }

            cache[indexName].Add(value);
            return await Task.FromResult(new SuccessfulResult());
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> UpdateConstraintAsync(string indexName, string oldValue, string newValue)
        {
            if (!cache.ContainsKey(indexName))
            {
                return new FailedResult($"indexName: {indexName} not registered");
            }

            if (!cache[indexName].Contains(oldValue))
            {
                return new FailedResult($"no such oldValue: {oldValue} for indexName: {indexName}");
            }

            if (cache[indexName].Contains(newValue))
            {
                return new FailedResult($"newValue: {newValue} for indexName: {indexName} already exists");
            }

            cache[indexName].Remove(oldValue);
            cache[indexName].Add(newValue);
            return await Task.FromResult(new SuccessfulResult());
        }
    }
}
