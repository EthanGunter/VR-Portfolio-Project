using UnityEngine;

namespace TowerDefenseVFXPack
{
    /// <summary>
    /// Make sure to remove this script from the prefab if you want to implement your own behaviour.
    /// </summary>
    public class DestroyParticleSystemOnCompletion : MonoBehaviour
    {
        /// <summary>
        /// Particle system must have Stop Action as Callback in the base settings for this to work.
        /// </summary>
        private void OnParticleSystemStopped()
        {
            Destroy(gameObject);
        }
    }
}