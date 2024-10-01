using System;
using System.Collections;
using System.Threading.Tasks;
using SolarStorm.UnityToolkit;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : PersistentSingleton<AudioManager>
{
    /// <summary>
    /// Volume multiplier from 0.0 -> 1.0
    /// </summary>
    public static float MasterVolume { get => _mVolume; set => _mVolume = Mathf.Clamp01(value); }
    private static float _mVolume = 1.0f;

    private ObjectPool<AudioSource> pool;


    protected override void Initialize()
    {
        base.Initialize();
        pool = new ObjectPool<AudioSource>(Instance.CreateSourcePoolObj, AudioSourceGet, AudioSourceRelease, AudioSourceDestroyed);
    }


    /// <summary>
    /// Plays a sound at a given location
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="position">The location that the audio should play at</param>
    /// <param name="volume">0-1 loudness multiplier</param>
    /// <returns>The audio source for further customization</returns>
    public static AudioSource Play(AudioClip clip, Vector3 position, float volume = 1)
    {
        AudioSource source = Instance.pool.Get();
        source.volume = Mathf.Clamp01(volume) * MasterVolume;
        source.transform.position = position;
        source.PlayOneShot(clip);

        Instance.ReturnWhenFinished(source);

        return source;
    }
    /// <summary>
    /// Plays a sound that follows a given object
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="boundTo">The transform of the object that the audio should be attached to</param>
    /// <param name="volume">0-1 loudness multiplier</param>
    /// <returns>The audio source for further customization</returns>
    public static AudioSource Play(AudioClip clip, Transform boundTo, float volume = 1)
    {
        AudioSource source = Instance.pool.Get();
        source.volume = Mathf.Clamp01(volume) * MasterVolume;
        source.transform.SetParent(boundTo);
        source.transform.localPosition = Vector3.zero;
        source.PlayOneShot(clip);

        Instance.ReturnWhenFinished(source);

        return source;
    }


    private AudioSource CreateSourcePoolObj()
    {
        GameObject obj = new GameObject("Pooled_AudioSource");
        obj.transform.SetParent(transform);
        AudioSource src = obj.AddComponent<AudioSource>();
        return src;
    }
    private void AudioSourceGet(AudioSource source)
    {
        source.gameObject.SetActive(true);
    }
    private void AudioSourceRelease(AudioSource source)
    {
        source.transform.SetParent(transform);
        source.gameObject.SetActive(false);
    }
    private void AudioSourceDestroyed(AudioSource source) { Destroy(source.gameObject); }


    private async void ReturnWhenFinished(AudioSource src)
    {
        while (src.isPlaying) { await Task.Delay(100); }
        pool.Release(src);
    }
}