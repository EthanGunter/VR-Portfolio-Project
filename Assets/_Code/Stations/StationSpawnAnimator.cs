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
            animTime = _animateTime * (1 - lastSpawnPercent);
            spawnDelay = (_animateTime - animTime) / itemsToAnimate.Count;
        }
    }
    float _animateTime = 2;
    [SerializeField] bool animateOnEnable = true;
    [SerializeField, Tooltip("or out")] bool animateIn = true;
    [SerializeField, Range(0, 1f), Tooltip("How far through the animation all objects will be spawned")] float lastSpawnPercent = .6f;
    [SerializeField] Vector3 targetOffset = new Vector3(0, 200, 0);
    [SerializeField] GameObject root;
    [SerializeField] List<Transform> itemsToAnimate = new();
    Dictionary<Transform, Vector3> originalHeights = new();
    float animTime, spawnDelay;


    private void Awake()
    {
        AnimateTime = _animateTime;
        foreach (Transform t in itemsToAnimate)
        {
            if (!animateOnEnable)
            {
                t.gameObject.SetActive(false);
            } 
            originalHeights.Add(t, t.position);
        }
    }

    private void OnEnable()
    {
        if (animateOnEnable)
        {
            if (animateIn)
            {
                AnimateIn();
            }
            else
            {
                AnimateOut();
            }
        }
    }

    public async Awaitable AnimateIn()
    {
        Queue<Transform> queue = new Queue<Transform>(itemsToAnimate);
        List<Task> animatingObjects = new();
        root?.SetActive(true);

        while (queue.Count > 0)
        {
            Transform child = queue.Dequeue();
            child.gameObject.SetActive(true);
            child.position += targetOffset;
            animatingObjects.Add(child.DOMoveY(originalHeights[child].y, animTime).SetEase(Ease.OutQuint).AsyncWaitForCompletion());

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
            Vector3 animTargetPos = child.position + targetOffset;
            animatingObjects.Add(child.DOMove(animTargetPos, animTime).SetEase(Ease.InQuint).OnComplete(() =>
            {
                child.transform.position = originalHeights[child];
                child.gameObject.SetActive(false);
            }).AsyncWaitForCompletion());

            await Awaitable.WaitForSecondsAsync(spawnDelay);
        }

        await Task.WhenAll(animatingObjects);
        root?.SetActive(false);
    }
}
