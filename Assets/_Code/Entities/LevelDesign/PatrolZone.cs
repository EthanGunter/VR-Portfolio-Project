using SolarStorm.UnityToolkit;
using UnityEngine;


namespace SolarStorm.LevelDesign
{
    public class PatrolZone : ZoneMarker
    {
        [Tooltip("The number of seconds AI will stay in this zone")]
        [SerializeField] private FloatRef _lingerTime = 5;
        [Tooltip("Higher values means less predictable wait times. 0 = Linger Time")]
        [SerializeField] private FloatRef _lingerTimeVariance = 0;
        [field: SerializeField] public PatrolZone[] Previous { get; private set; }
        [field: SerializeField] public PatrolZone[] Next { get; private set; }

        public PatrolZone GetNextZone()
        {
            return Next[Random.Range(0, Next.Length)];
        }
        public float GetLingerTime()
        {
            return Random.Range(_lingerTime - _lingerTimeVariance, _lingerTime + _lingerTimeVariance);
        }
    }
}