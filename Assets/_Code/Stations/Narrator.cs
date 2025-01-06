using DG.Tweening;
using Sirenix.OdinInspector;
using SolarStorm.UnityToolkit;
using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
public class Narrator : SerializedMonoBehaviour
{
    #region STATIC

    private static Narrator instance
    {
        get
        {
            if (Application.isPlaying && _instance == null)
            {
                _instance = FindAnyObjectByType<Narrator>();
                if (_instance == null)
                {
                    _instance = new GameObject(nameof(Narrator)).AddComponent<Narrator>();
                }
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    private static Narrator _instance;

    public static void Narrate(AudioClip voiceover, Vector3 location = default)
    {
        instance.NarrateClip(voiceover, location);
    }

    #endregion


    [SerializeField] Gradient gradient;
    [SerializeField, MinMaxSlider(-80, 0)] Vector2 gradientDBBounds = new Vector2(-80f, 0f);
    [SerializeField, Range(0, 1)] float smoothness = 0;
    AudioSource audioSource;
    new SpriteRenderer renderer;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = gradient.Evaluate(0);
    }

    [Button]
    public async Task NarrateClip(AudioClip voiceover, Vector3 location = default)
    {
        if (location != default)
        {
            instance.transform.DOMove(location, 2).SetEase(Ease.OutCubic);
        }
        if (audioSource == null) return;

        int sampleSize = 512;
        float[] audioSamples = new float[sampleSize];

        audioSource.clip = voiceover;
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            audioSource.GetOutputData(audioSamples, 0);
            // Calculate the RMS amplitude
            float sum = 0f;
            for (int i = 0; i < sampleSize; i++)
            {
                sum += audioSamples[i] * audioSamples[i];
            }

            float rms = Mathf.Sqrt(sum / sampleSize);

            // Convert RMS to dB
            float db = 20 * Mathf.Log10(rms);

            // Clamp the dB to a reasonable range to avoid negative infinity
            db = Mathf.Clamp(db, -80f, 0f);

            // Scale to get %
            float intensity = (db - gradientDBBounds.x) / (gradientDBBounds.y - gradientDBBounds.x);

            // Set the sprite color
            renderer.color = Color.Lerp(renderer.color, gradient.Evaluate(intensity), 1 - smoothness);

            await Awaitable.EndOfFrameAsync();
        }

        // Reset for next clip
        audioSource.clip = null;
        renderer.color = gradient.Evaluate(0);
    }
}
