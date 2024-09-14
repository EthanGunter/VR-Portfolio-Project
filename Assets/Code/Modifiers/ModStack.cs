using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarStorm.Modifiers
{
    /// <summary>
    /// An advanced ModList that allows the propagation of complex info in the form of <see cref="IModifierContext{T}"/>s
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModifierStack<T>
    {
        private readonly List<ModifierFunc<T>> _mods = new();

        /// <summary>
        /// Processes the 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public T Apply(IModifierContext<T> context)
        {
            for (int i = 0; i < _mods.Count; i++)
            {
                context = _mods[i].Invoke(context);
            }
            return context.Value;
        }

        public int ModCount => _mods.Count;

        /// <summary></summary>
        /// <param name="modifier">A function that takes an IModifierContext<T> in, then returns IModifierContext<T> in a modified state</param>
        /// <param name="duration">A duration of 0 or less will be indefinite</param>
        /// <returns>In the event duration > 0, this awaitable will return after the modifier has been removed</returns>
        public async Awaitable Add(ModifierFunc<T> modifier, float duration = -1)
        {
            if (duration > 0)
            {
                _mods.Add(modifier);

                await RunAfter(() => { Remove(modifier); }, duration);
            }
            else
            {
                _mods.Add(modifier);
            }
        }

        public void Clear()
        {
            _mods.Clear();
        }

        public bool Remove(ModifierFunc<T> modifier)
        {
            return _mods.Remove(modifier);
        }

        private async Awaitable RunAfter(Action action, float seconds)
        {
            await Awaitable.WaitForSecondsAsync(seconds);
            action();
        }
    }
}