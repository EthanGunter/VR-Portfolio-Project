using DG.Tweening;
using SolarStorm.UnityToolkit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;

public class StationSpawnAnimator : MonoBehaviour
{
    [SerializeField] float animateTime = 2;
    [SerializeField, Range(0, 1f), Tooltip("How far through the animation all objects will be spawned")] float lastSpawnPercent = .6f;
    [SerializeField] float fallHeight = 200;
    [SerializeField] List<Transform> itemsToAnimate = new();
    [SerializeField] Dictionary<Transform, Vector3> originalHeights = new();
    float fallTime, spawnDelay;


    private void Awake()
    {
        fallTime = animateTime * (1 - lastSpawnPercent);
        spawnDelay = (animateTime - fallTime) / itemsToAnimate.Count;

        foreach (Transform t in itemsToAnimate)
        {
            t.gameObject.SetActive(false);
            originalHeights.Add(t, t.position);
        }
    }

    public async Task AnimateIn()
    {
        Queue<Transform> queue = new Queue<Transform>(itemsToAnimate);
        while (queue.Count > 0)
        {
            Transform child = queue.Dequeue();
            child.gameObject.SetActive(true);
            child.position += Vector3.up * fallHeight;
            child.DOMoveY(originalHeights[child].y, fallTime).SetEase(Ease.OutQuint);

            await Awaitable.WaitForSecondsAsync(spawnDelay);
        }
    }
    public async Task AnimateOut()
    {
        Stack<Transform> queue = new Stack<Transform>(itemsToAnimate);
        while (queue.Count > 0)
        {
            Transform child = queue.Pop();
            float animTargetY = child.position.y + fallHeight;
            child.DOMoveY(animTargetY, fallTime).SetEase(Ease.InQuint).OnComplete(() =>
            {
                child.transform.position = originalHeights[child];
                child.gameObject.SetActive(false);
            });

            await Awaitable.WaitForSecondsAsync(spawnDelay);
        }
    }
}
