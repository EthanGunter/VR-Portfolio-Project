using System;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;

public class ActionSortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)dictionary).Keys;
    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)dictionary).Values;
    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count;
    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;
    public TValue this[TKey key]
    {
        get => ((IDictionary<TKey, TValue>)dictionary)[key];
        set
        {
            dictionary[key] = value;
            SortKeys();
        }
    }

    private Dictionary<TKey, TValue> dictionary = new();
    private List<TKey> sortedKeys = new();
    private readonly Comparison<KeyValuePair<TKey, TValue>> comparer;


    public ActionSortedDictionary(Comparison<KeyValuePair<TKey, TValue>> sortingFunction)
    {
        comparer = sortingFunction;
    }


    public void Add(TKey key, TValue value)
    {
        if (dictionary.ContainsKey(key))
            throw new ArgumentException("Key already exists");

        dictionary[key] = value;
        sortedKeys.Add(key);
        SortKeys();
    }
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }
    public bool Remove(TKey key)
    {
        if (!dictionary.Remove(key))
            return false;

        sortedKeys.Remove(key);
        return true;
    }
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }
    public bool RemoveLast()
    {
        return Remove(sortedKeys[Count - 1]);
    }
    public bool RemoveFirst()
    {
        return Remove(sortedKeys[0]);
    }
    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
    public IEnumerable<KeyValuePair<TKey, TValue>> GetSortedItems(bool reverse = false)
    {
        if (reverse)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                TKey key = sortedKeys[i];
                yield return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
            }
        }
        else
        {
            for (int i = 0; i < Count; i++)
            {
                TKey key = sortedKeys[i];
                yield return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
            }
        }
    }
    private void SortKeys()
    {
        sortedKeys.Sort((a, b) =>
        {
            var val = comparer(
            new KeyValuePair<TKey, TValue>(a, dictionary[a]),
            new KeyValuePair<TKey, TValue>(b, dictionary[b]));
            return val;
        });
    }

    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)dictionary).ContainsKey(key);
    }
    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear();
    }
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
    }
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
    }
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)dictionary).GetEnumerator();
    }
}
