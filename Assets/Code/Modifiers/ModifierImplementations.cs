using UnityEngine;


namespace SolarStorm.Modifiers
{
    public class Modifier<T>
    {
        public readonly string Name;
        public readonly ModifierFunc<T> modifierFunc;
    }

    public static class Modifiers
    {
        public static ModifierFunc<float> InvertValue = (IModifierContext<float> context) =>
        {
            context.Value = -context.Value;
            return context;
        };
        public static ModifierFunc<float> MakeZero = (IModifierContext<float> context) =>
        {
            context.Value = 0;
            return context;
        };
    }
}
