using System;
using System.Collections;
using System.Collections.Generic;

namespace SolarStorm.DataStructures
{
    public class WeightedList<T>: IList<T>
    {
        private Random rand = new Random();
        private List<T> list = new List<T>();
        private Dictionary<T, float> bag = new Dictionary<T, float>();
        private float total = 0;

        public int Count => ((ICollection<T>)list).Count;
        public bool IsReadOnly => ((ICollection<T>)list).IsReadOnly;
        public T this[int index] { get => ((IList<T>)list)[index]; set => ((IList<T>)list)[index] = value; }

        public T GetRandom()
        {
            float val = (float)rand.NextDouble() * total;
            foreach (var item in bag)
            {
                if (val < item.Value)
                    return item.Key;

                val -= item.Value;
            }

            return default;
        }

        public void Remove(T item)
        {

            if (bag.ContainsKey(item))
            {
                total -= bag[item];
                bag.Remove(item);
            }
        }
        public int IndexOf(T item)
        {
            return ((IList<T>)list).IndexOf(item);
        }
        public void Insert(int index, T item)
        {
            ((IList<T>)list).Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            ((IList<T>)list).RemoveAt(index);
        }
        public void Add(T item)
        {
            Add(item, 0);
        }
        public void Add(T item, float weight = 1)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight must be greater than 0");

            list.Add(item);
            if (bag.ContainsKey(item))
            {
                bag[item] += weight;
                total += weight;
            }
            else
            {
                bag.Add(item, weight);
                total += weight;
            }

        }
        public void Clear()
        {
            ((ICollection<T>)list).Clear();
        }
        public bool Contains(T item)
        {
            return ((ICollection<T>)list).Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)list).CopyTo(array, arrayIndex);
        }
        bool ICollection<T>.Remove(T item)
        {
            return ((ICollection<T>)list).Remove(item);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)list).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}
