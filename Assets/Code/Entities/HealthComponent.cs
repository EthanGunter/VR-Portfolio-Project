using SolarStorm.UnityToolkit;
using SolarStorm.Modifiers;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace SolarStorm.Entities
{
    public class HealthComponent : EntityComponent
    {
        public event Action<float> OnDamaged;
        public event Action<float> OnHealed;
        public event Action Killed;

        [field: SerializeField] public FloatRef MaxHealth { get; private set; } = 100;
        [field: SerializeField] public float Health { get; protected set; }

        public bool Dead;

        private readonly ModifierStack<float> damageModifiers = new();

        protected override void Awake()
        {
            base.Awake();
            Health = MaxHealth.Value;
        }

        public void Initialize()
        {
            Health = MaxHealth;
        }

        public void AddDamageModifier(ModifierFunc<float> modifier, float duration = -1)
        {
            damageModifiers.Add(modifier, duration);
        }
        public void RemoveDamageModifier(ModifierFunc<float> modifier)
        {
            damageModifiers.Remove(modifier);
        }
        public virtual void DealDamage(DamageContext damage)
        {
            damage.Target = Entity;
            damageModifiers.Apply(damage);
            Health -= damage.Value;
            OnDamaged?.Invoke(damage.Value);
            if (!Dead && Health <= 0)
            {
                Dead = true;
                Killed?.Invoke();
            }
        }
    }

    public class DamageContext : IModifierContext<float>
    {
        public static implicit operator DamageContext(float value) => new DamageContext(value);

        public float Value { get; set; }
        public GameEntity Target { get; set; }
        public GameEntity Source { get; set; } = null;


        public DamageContext() { }
        public DamageContext(float amount, GameEntity source = null)
        {
            Value = amount;
            Source = source;
        }
    }
}
