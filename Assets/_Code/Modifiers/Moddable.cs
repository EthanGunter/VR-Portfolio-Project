using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SolarStorm.Modifiers
{
    /// <summary>
    /// Allows any type T to have modifiers applied to it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Moddable<T>
    {
        /// <summary>
        /// The underlying value. Should be constant, as reading and writing to this are unaffected by modifiers
        /// </summary>
        public T BaseValue { get; set; }
        private readonly List<Func<T, T>> _list = new List<Func<T, T>>();
        public int ModCount => _list.Count;


        public Moddable(T baseValue)
        {
            BaseValue = baseValue;
        }


        /// <param name="modifier">A simple func that takes T in, then returns T in a modified state</param>
        /// <param name="duration">A duration of 0 or less will be indefinite</param>
        /// <returns>In the event duration > 0, this awaitable will return after the modifier has been removed, otherwise it is synchronous</returns>
        public async void AddModifier(Func<T, T> modifier, float duration = -1)
        {
            if (duration > 0)
            {
                _list.Add(modifier);

                await RunAfter(() => { RemoveModifier(modifier); }, duration);
            }
            else
            {
                _list.Add(modifier);
            }
        }

        /// <param name="modifier">The modifier func to remove</param>
        /// <returns>Whether or not the removal was successful</returns>
        public bool RemoveModifier(Func<T, T> modifier)
        {
            return _list.Remove(modifier);
        }

        /// <summary>
        /// Removes all modifiers
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }


        /// <summary>
        /// Processes the effect of each modifier on the base value
        /// </summary>
        /// <param name="value">The base value</param>
        /// <returns>The modified value</returns>
        private T Apply(T value)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                value = _list[i].Invoke(value);
            }
            return value;
        }

        private async Awaitable RunAfter(Action action, float seconds)
        {
            await Awaitable.WaitForSecondsAsync(seconds);
            action();
        }


        public static implicit operator T(Moddable<T> moddable) => moddable.Apply(moddable.BaseValue);
        public override string ToString() => Apply(BaseValue).ToString();
    }
}
