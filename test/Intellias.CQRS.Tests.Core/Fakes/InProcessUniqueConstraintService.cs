using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
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
                var customMessage = $"indexName: {indexName} not registered";

                return FailedResult.Create(
                        CoreErrorCodes.UnhandledError,
                        CoreErrorCodes.NameIsNotFound,
                        customMessage);
            }

            if (!cache[indexName].Contains(value))
            {
                var customMessage = $"no such value: {value} for indexName: {indexName}";

                return FailedResult.Create(
                        CoreErrorCodes.UnhandledError,
                        CoreErrorCodes.NameIsNotFound,
                        customMessage);
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
                var customMessage = $"value: {value} for indexName: {indexName} already exists";

                return FailedResult.Create(
                        CoreErrorCodes.ValidationFailed,
                        CoreErrorCodes.NameIsInUse,
                        customMessage);
            }

            cache[indexName].Add(value);
            return await Task.FromResult(new SuccessfulResult());
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> UpdateConstraintAsync(string indexName, string oldValue, string newValue)
        {
            if (!cache.ContainsKey(indexName))
            {
                var notInUseMessage = $"indexName: {indexName} not registered";

                return FailedResult.Create(
                        CoreErrorCodes.UnhandledError,
                        CoreErrorCodes.NameIsNotFound,
                        notInUseMessage);
            }

            if (!cache[indexName].Contains(oldValue))
            {
                var notInUseMessage = $"no such oldValue: {oldValue} for indexName: {indexName}";

                return FailedResult.Create(
                        CoreErrorCodes.UnhandledError,
                        CoreErrorCodes.NameIsNotFound,
                        notInUseMessage);
            }

            if (cache[indexName].Contains(newValue))
            {
                var inUseMessage = $"newValue: {newValue} for indexName: {indexName} already exists";

                return FailedResult.Create(
                        CoreErrorCodes.ValidationFailed,
                        CoreErrorCodes.NameIsInUse,
                        inUseMessage);
            }

            cache[indexName].Remove(oldValue);
            cache[indexName].Add(newValue);
            return await Task.FromResult(new SuccessfulResult());
        }
    }
}
