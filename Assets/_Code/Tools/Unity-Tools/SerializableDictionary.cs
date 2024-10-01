using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary = new();


        public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)_dictionary)[key]; set => ((IDictionary<TKey, TValue>)_dictionary)[key] = value; }

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dictionary).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dictionary).Values;

        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

        public void Add(TKey key, TValue value) => ((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
        public void Clear() => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
        public bool ContainsKey(TKey key) => ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
        public bool Remove(TKey key) => ((IDictionary<TKey, TValue>)_dictionary).Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
        public bool TryGetValue(TKey key, out TValue value) => ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();
    }
}
