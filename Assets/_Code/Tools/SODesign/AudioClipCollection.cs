using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Collection/Audio Clips")]
    public class AudioClipCollection : ScriptableObject
    {
        public AudioClip[] audioClips;
    }
}
