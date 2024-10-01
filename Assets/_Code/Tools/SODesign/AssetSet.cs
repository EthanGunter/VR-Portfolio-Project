using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SolarStorm.UnityToolkit
{
    public abstract class AssetSet<T> : ScriptableObject
    {
        [SerializeField] private readonly List<T> _items;
        private int _count;


        public bool IsReadOnly => ((ICollection<T>)_items).IsReadOnly;

        public T GetRandom()
        {
            return _items[Random.Range(0, _items.Count)];
        }

        public T FindFirst(Func<T, bool> predicate = null)
        {
            T item;
            if (predicate != null)
                item = _items.FirstOrDefault(predicate);
            else
                item = _items.FirstOrDefault();

            return item;
        }
        public IEnumerable<T> FindAll(Func<T, bool> predicate = null)
        {
            return _items.Where(predicate);
        }
    }
}
