
using SolarStorm.Modifiers;
using System;
using UnityEngine;

namespace SolarStorm.Entities
{
    [RequireComponent(typeof(TrainingDummy))]
    public class TrainingDummyView : MonoBehaviour
    {
        #region Variables

        [SerializeField] Animator _anim;
        private TrainingDummy _model;
        private HealthComponent _health;

        #endregion


        #region Unity Messages

        private void Awake()
        {
            _model = GetComponent<TrainingDummy>();
            _health = GetComponent<HealthComponent>();
            _health.OnDamaged += Health_OnDamaged;
            _health.OnKilled += _health_OnKilled;
            _health.AddDamageModifier(Modifiers.Modifiers.MakeZero); // Invincibility
        }

        #endregion


        private void Health_OnDamaged(float obj)
        {
            _anim.SetTrigger("hurt");
        }
        private void _health_OnKilled()
        {
            _anim.SetTrigger("dead");
        }
    }
}