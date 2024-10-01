using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [CreateAssetMenu(menuName = "Shared Data/Asset Collections/Audio Clips")]
    public class AudioClipCollection : ScriptableObject
    {
        public AudioClip[] audioClips;
    }
}
