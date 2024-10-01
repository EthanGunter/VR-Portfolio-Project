using Barmetler;
using Barmetler.RoadSystem;
using DG.Tweening;
using SolarStorm.UnityToolkit;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FollowRoad : MonoBehaviour
{
    [SerializeField] Road road;
    [SerializeField] UnityEvent EndOfRoadReached;
    [SerializeField] FloatRef moveSpeed = 1;
    [SerializeField] FloatRef pointSpacing = 1;

    [SerializeField] Vector3 offset;

    private Tween moveTween;
    [SerializeField] private Bezier.OrientedPoint[] path;

    private void Start()
    {
        StartFollow();
    }

    public async void StartFollow()
    {
        path = road.GetEvenlySpacedPoints(pointSpacing).Select(e => e.ToWorldSpace(road.transform)).ToArray();
        for (int j = 0; j < path.Length; j++)
        {
            Vector3 point = path[j].position + offset;
            moveTween = transform.DOMove(point, moveSpeed).SetSpeedBased().SetEase(Ease.Linear);
            await moveTween.AsyncWaitForCompletion();
        }
        if (path.Length != 0)
        {
            EndOfRoadReached?.Invoke();
        }
    }
    public void StopFollow()
    {
        moveTween.Kill();
        path = new Bezier.OrientedPoint[0];
    }
}
