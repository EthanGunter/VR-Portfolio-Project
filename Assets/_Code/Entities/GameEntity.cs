using System;
using System.Collections.Generic;
using UnityEngine;
using SolarStorm.UnityToolkit;
using SolarStorm.LevelDesign;
using System.Linq;

namespace SolarStorm.Entities
{
    /// <summary>
    /// The base class of all "living" objects
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public abstract class GameEntity : MonoBehaviour
    {
        #region Static Members

        private static Dictionary<Type, HashSet<GameEntity>> _allEntities = new();

        public static T FindFirst<T>(Func<GameEntity, bool> predicate = null) where T : GameEntity
        {
            if (_allEntities.TryGetValue(typeof(T), out var tEntities))
            {
                if (predicate != null)
                    return (T)tEntities.First(predicate);
                else return (T)tEntities.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        public static IEnumerable<T> FindAll<T>(Func<GameEntity, bool> predicate = null) where T : GameEntity
        {
            if (_allEntities.TryGetValue(typeof(T), out var tEntities))
            {
                if (predicate != null)
                    return (IEnumerable<T>)tEntities.Where(predicate).ToList();
                else return (IEnumerable<T>)tEntities;
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region Instance Members

        [SerializeField] private GameEntityRuntimeSet _belongsTo;

        public virtual void MoveTo(Vector3 position)
        {
            transform.position = position;
        }
        #endregion


        #region Unity Message Virtualization

        /// <summary>
        /// Adds this entity to its respective runtime sets
        /// </summary>
        protected virtual void Awake()
        {
            Type type = GetType();
            if (_allEntities.TryGetValue(type, out HashSet<GameEntity> list))
            {
                list.Add(this);
            }
            else
            {
                _allEntities.Add(GetType(), new HashSet<GameEntity>() { this });
            }
        }
        protected virtual void OnEnable()
        {
            if (_belongsTo != null) { _belongsTo.Add(this); }
        }
        protected virtual void FixedUpdate() { }
        protected virtual void Update() { }
        protected virtual void LateUpdate() { }
        protected virtual void OnDisable()
        {
            if (_belongsTo != null) { _belongsTo.Remove(this); }
        }
        protected virtual void OnDestroy()
        {
            _allEntities[GetType()].Remove(this);
        }

        #endregion
    }
}
