using SolarStorm.Modifiers;
using UnityEngine;


namespace SolarStorm.Entities
{
    public abstract class EntityModifier : ScriptableObject
    {
        /// <summary>
        /// The name to shows in the UI if a player inspects an entity's modifiers
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// A unique identifier in the event of stacking modifiers. Defaults to <see cref="DisplayName"/>
        /// </summary>
        public virtual string ID => DisplayName;

        /// <returns>
        /// true if this modifier can be applied multiple times on the same <see cref="GameObject"/>
        /// </returns>
        public abstract bool CanStack { get; }

        /// <param name="target"></param>
        /// <returns>True if the target</returns>
        public abstract bool CanApplyTo(GameObject target);
        public abstract void OnEffectStart(GameObject target);
        public abstract void OnEffectEnd(GameObject target);
    }
}