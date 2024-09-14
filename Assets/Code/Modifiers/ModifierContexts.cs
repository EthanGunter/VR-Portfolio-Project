using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarStorm.Modifiers
{
    public interface IModifierContext<T>
    {
        public T Value { get; set; }
    }

    public delegate IModifierContext<T> ModifierFunc<T>(IModifierContext<T> context);
}
