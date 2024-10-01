using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.XR.CoreUtils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SolarStorm.UnityToolkit
{
    public abstract class RuntimeSet<T> : ScriptableObject, ICollection<T>
    {
        [SerializeField] private readonly Dictionary<Type, HashSet<T>> _items = new();
        private int _count;

        #region ICollection

        public int Count => _count;
        public bool IsReadOnly => ((ICollection<T>)_items).IsReadOnly;

        public void Add(T item)
        {
            Type type = item.GetType();
            if (_items.TryGetValue(type, out HashSet<T> set))
            {
                set.Add(item);
            }
            else
            {
                _items.Add(item.GetType(), new HashSet<T>() { item });
            }
            _count++;
        }
        public bool Remove(T item)
        {
            _count--;
            return _items[item.GetType()].Remove(item);
        }
        public T GetRandom()
        {
            return _items.ElementAt(Random.Range(0, _items.Count)) // Get random subtype
                .Value.ElementAt(Random.Range(0, _items.Count)); // then random item from that hashset
        }
        public T GetRandom<Subtype>()
        {
            return _items[typeof(Subtype)].ElementAt(Random.Range(0, _items.Count));
        }
        public void Clear()
        {
            _items.Clear();
            _count = 0;
        }

        public bool Contains(T item)
        {
            foreach (var set in _items.Values)
            {
                if (set.Contains(item)) return true;
            }
            return false;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        // TODO this could be more efficient
        public IEnumerator<T> GetEnumerator()
        {
            List<T> values = new();
            foreach (var set in _items.Values)
            {
                values.AddRange(set);
            }
            return values.GetEnumerator();
        }
        // TODO this could be more efficient by writing a custom enumerator...
        IEnumerator IEnumerable.GetEnumerator()
        {
            List<T> values = new();
            foreach (var set in _items.Values)
            {
                values.AddRange(set);
            }
            return values.GetEnumerator();
        }

        #endregion


        public T FindFirst(Func<T, bool> predicate = null)
        {
            foreach (var set in _items.Values)
            {
                T item;
                if (predicate != null)
                    item = set.FirstOrDefault(predicate);
                else
                    item = set.FirstOrDefault();

                if (!item.Equals(default(T)))
                    return item;
            }
            return default;
        }
        public T FindFirst<Subtype>(Func<T, bool> predicate = null)
        {
            if (_items.TryGetValue(typeof(Subtype), out var set))
            {
                if (predicate != null)
                    return (T)set.First(predicate);
                else return (T)set.FirstOrDefault();
            }
            else
            {
                Debug.LogError($"{name} contains no objects with subtype: {typeof(Subtype).Name}", this);
                return default;
            }
        }
        public IEnumerable<T> FindAll(Func<T, bool> predicate = null)
        {
            List<T> list = new();
            if (predicate != null)
            {
                foreach (var set in _items.Values)
                {
                    list.AddRange(set.Where(predicate));
                }
            }
            else
            {
                foreach (var set in _items.Values)
                {
                    list.AddRange(set);
                }
            }
            return list;
        }
        public IEnumerable<T> FindAll<Subtype>(Func<T, bool> predicate = null)
        {
            if (_items.TryGetValue(typeof(Subtype), out var set))
            {
                if (predicate != null)
                    return (IEnumerable<T>)set.Where(predicate).ToList();
                else return (IEnumerable<T>)set;
            }
            else
            {
                Debug.LogError($"{name} contains no objects with subtype: {typeof(Subtype).Name}", this);
                return default;
            }
        }
    }
}
