using SolarStorm.Entities;
using UnityEditor;
using UnityEngine;

namespace SolarStorm.LevelDesign
{
    [RequireComponent(typeof(BoxCollider))]
    public class SpawnZone : ZoneMarker
    {
        [SerializeField] private SpawnZoneCollection _belongsTo;

        protected override void Awake()
        {
            base.Awake();
            if (_belongsTo != null) { _belongsTo.Add(this); }
        }
        private void OnDestroy()
        {
            if (_belongsTo != null) { _belongsTo.Remove(this); }
        }

        /// <summary>
        /// Places an entity at a random location within the designated spawn zone colliders
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="spawnAtZoneOrigin"></param>
        public void PlaceEntity(GameEntity entity)
        {
            if (entity == null) return;

            Vector3 point = GetRandomPointInZone();
            entity.SetPosition(point);
        }
    }
}
