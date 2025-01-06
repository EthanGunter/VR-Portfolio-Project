using DG.Tweening;
using Sirenix.OdinInspector;
using SolarStorm.UnityToolkit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;

public class StationSpawnAnimator : SerializedMonoBehaviour
{
    [DelayedProperty]
    float AnimateTime
    {
        get { return _animateTime; }
        set
        {
            _animateTime = value;
            fallTime = _animateTime * (1 - lastSpawnPercent);
            spawnDelay = (_animateTime - fallTime) / itemsToAnimate.Count;
        }
    }
    float _animateTime = 2;
    [SerializeField] bool spawnOnStart = false;
    [SerializeField, Range(0, 1f), Tooltip("How far through the animation all objects will be spawned")] float lastSpawnPercent = .6f;
    [SerializeField] float fallHeight = 200;
    [SerializeField] List<Transform> itemsToAnimate = new();
    [SerializeField] Dictionary<Transform, Vector3> originalHeights = new();
    float fallTime, spawnDelay;


    private void Awake()
    {
        AnimateTime = _animateTime;
        foreach (Transform t in itemsToAnimate)
        {
            if (!spawnOnStart)
            {
                t.gameObject.SetActive(false);
            }
            originalHeights.Add(t, t.position);
        }
    }

    public async Awaitable AnimateIn()
    {
        Queue<Transform> queue = new Queue<Transform>(itemsToAnimate);
        List<Task> animatingObjects = new();

        while (queue.Count > 0)
        {
            Transform child = queue.Dequeue();
            child.gameObject.SetActive(true);
            child.position += Vector3.up * fallHeight;
            animatingObjects.Add(child.DOMoveY(originalHeights[child].y, fallTime).SetEase(Ease.OutQuint).AsyncWaitForCompletion());

            await Awaitable.WaitForSecondsAsync(spawnDelay);
        }

        await Task.WhenAll(animatingObjects);
    }
    public async Awaitable AnimateOut()
    {
        Stack<Transform> queue = new Stack<Transform>(itemsToAnimate);
        List<Task> animatingObjects = new();

        while (queue.Count > 0)
        {
            Transform child = queue.Pop();
            float animTargetY = child.position.y + fallHeight;
            animatingObjects.Add(child.DOMoveY(animTargetY, fallTime).SetEase(Ease.InQuint).OnComplete(() =>
            {
                child.transform.position = originalHeights[child];
                child.gameObject.SetActive(false);
            }).AsyncWaitForCompletion());

            await Awaitable.WaitForSecondsAsync(spawnDelay);
        }

        await Task.WhenAll(animatingObjects);
    }
}
