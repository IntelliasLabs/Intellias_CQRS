using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.QueryStore.AzureTable.Common;
using Newtonsoft.Json;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessTableStorage<T> : IList<T>, IReadOnlyCollection<T>
    {
        private readonly List<string> storage = new List<string>();

        public int Count => storage.Count;

        public bool IsReadOnly => false;

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

        public bool Contains(T item) =>
            Deserialize(storage).Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            Deserialize(storage).ToList().CopyTo(array, arrayIndex);

        public bool Remove(T item) =>
            storage.RemoveAll(e => e == Serialize(item)) > 0;

        public int IndexOf(T item) =>
            storage.IndexOf(Serialize(item));

        public void Insert(int index, T item) =>
            storage.Insert(index, Serialize(item));

        public void RemoveAt(int index) =>
            storage.RemoveAt(index);

        private static string Serialize(object? entity) =>
            JsonConvert.SerializeObject(entity, TableStorageJsonSerializerSettings.GetDefault());

        private static IEnumerable<T> Deserialize(IEnumerable<string> storage) =>
            storage.Select(e => JsonConvert.DeserializeObject<T>(e, TableStorageJsonSerializerSettings.GetDefault()));
    }
}