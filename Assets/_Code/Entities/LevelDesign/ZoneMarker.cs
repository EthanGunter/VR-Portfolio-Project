using SolarStorm.DataStructures;
using SolarStorm.Entities;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SolarStorm.LevelDesign
{
    public class ZoneMarker : MonoBehaviour
    {
        [SerializeField] Collider[] colliders;

        public event Action<GameEntity> GameEntityEnteredZone;
        public event Action<GameEntity> GameEntityLeftZone;
        protected WeightedList<Collider> _zone = new WeightedList<Collider>();

        /// <summary>
        /// Puts all colliders into a list, weighted by their volume
        /// <para>Note: This also forces all colliders to be triggers</para>
        /// </summary>
        protected virtual void Awake()
        {
            foreach (Collider c in colliders)
            {
                c.isTrigger = true;
                float volume = c.bounds.size.x * c.bounds.size.y * c.bounds.size.z;
                _zone.Add(c, volume);
            }
        }

        public Vector3 GetRandomPointInZone(bool ignoreZ = false)
        {
            Collider col = _zone.GetRandom();
            float x = UnityEngine.Random.Range(col.bounds.min.x, col.bounds.max.x);
            float y = UnityEngine.Random.Range(col.bounds.min.y, col.bounds.max.y);
            float z = ignoreZ ? col.transform.position.z : UnityEngine.Random.Range(col.bounds.min.z, col.bounds.max.z);
            Vector3 point = new Vector3(x, y, z);
            return col.ClosestPoint(point);
        }

        /// <summary>
        /// Capture 2d/3d trigger events and raise an event
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            TriggerEnter(collider.gameObject);
        }
        protected virtual void OnTriggerEnter(Collider collider)
        {
            TriggerEnter(collider.gameObject);
        }
        protected virtual void TriggerEnter(GameObject collider)
        {
            if (collider.TryGetComponent(out GameEntity character))
            {
                GameEntityEnteredZone?.Invoke(character);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            TriggerExit(collider.gameObject);
        }
        protected virtual void OnTriggerExit(Collider collider)
        {
            TriggerExit(collider.gameObject);
        }
        protected virtual void TriggerExit(GameObject collider)
        {
            if (collider.TryGetComponent(out GameEntity character))
            {
                GameEntityLeftZone?.Invoke(character);
            }
        }
    }
}
