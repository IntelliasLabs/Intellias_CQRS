using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Newtonsoft.Json;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessTableStorage<T> : IReadOnlyList<T>
        where T : class
    {
        private List<string> storage = new List<string>();

        public int Count => storage.Count;

        public T this[int index]
        {
            get => Deserialize(storage).ElementAt(index);
            set => storage[index] = Serialize(value);
        }

        public IEnumerator<T> GetEnumerator() =>
            Deserialize(storage).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public void Add(T item) =>
            storage.Add(Serialize(item));

        public void Clear() =>
            storage.Clear();

        public void Insert(int index, T item) =>
            storage.Insert(index, Serialize(item));

        public void RemoveAt(int index) =>
            storage.RemoveAt(index);

        public void RemoveAll(Func<T, bool> predicate) =>
            storage = this.Where(e => !predicate(e)).Select(Serialize).ToList();

        private static string Serialize(object entity) =>
            JsonConvert.SerializeObject(entity, TableStorageJsonSerializerSettings.GetDefault());

        private static IEnumerable<T> Deserialize(IEnumerable<string> storage) =>
            storage.Select(e => JsonConvert.DeserializeObject<T>(e, TableStorageJsonSerializerSettings.GetDefault()));
    }
}