using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SolarStorm.UnityToolkit
{
    public abstract class AssetSet<T> : ScriptableObject
    {
        [SerializeField] protected List<T> items;

        public bool IsReadOnly => ((ICollection<T>)items).IsReadOnly;

        public T GetRandom()
        {
            return items[Random.Range(0, items.Count)];
        }

        public T FindFirst(Func<T, bool> predicate = null)
        {
            T item;
            if (predicate != null)
                item = items.FirstOrDefault(predicate);
            else
                item = items.FirstOrDefault();

            return item;
        }
        public IEnumerable<T> FindAll(Func<T, bool> predicate = null)
        {
            return items.Where(predicate);
        }
    }
}
