using SolarStorm.UnityToolkit;
using SolarStorm.Modifiers;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem.iOS;

namespace SolarStorm.Entities
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] UnityEvent EntityKilled;
        public event Action OnKilled;
        void InvokeKilled() { OnKilled?.Invoke(); EntityKilled.Invoke(); }

        public event Action<float> OnDamaged;
        public event Action<float> OnHealed;

        [field: SerializeField] public FloatRef MaxHealth { get; private set; } = 100;
        public float Health
        {
            get => _health;
            protected set
            {
                _health = value;
                if (IsAlive && value <= 0)
                {
                    IsAlive = false;
                    InvokeKilled();
                }
            }
        }
        [SerializeField] private float _health; // Serialized for debugging only
        public bool IsAlive { get; protected set; } = true;


        private readonly ModifierStack<float> damageModifiers = new();

        protected void Awake()
        {
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
            damage.Target = gameObject;
            damageModifiers.Apply(damage);

            Health -= damage.Value;
            OnDamaged?.Invoke(damage.Value);
        }
    }

    public class DamageContext : IModifierContext<float>
    {
        public static implicit operator DamageContext(float value) => new DamageContext(value);

        public float Value { get; set; }
        public GameObject Target { get; set; }
        public GameObject Source { get; set; } = null;


        public DamageContext() { }
        public DamageContext(float amount, GameObject source = null)
        {
            Value = amount;
            Source = source;
        }
    }
}
