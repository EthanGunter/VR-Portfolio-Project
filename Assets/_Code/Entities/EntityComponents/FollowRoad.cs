using Barmetler;
using Barmetler.RoadSystem;
using DG.Tweening;
using SolarStorm.UnityToolkit;
using System.Collections.Generic;
using System.IO;
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

    [SerializeField] float heightOffset;

    private Tween moveTween;
    [SerializeField] private Bezier.OrientedPoint[] path;


    public async void StartFollow(Road road = null, Vector3 offset = default)
    {
        offset.y += heightOffset;
        path = road.GetEvenlySpacedPoints(pointSpacing).Select(e => e.ToWorldSpace(road.transform)).ToArray();
        for (int j = 0; j < path.Length; j++)
        {
            Vector3 point = path[j].position + offset;
            moveTween = transform.DOMove(point, moveSpeed).SetSpeedBased().SetEase(Ease.Linear);
            await moveTween.AsyncWaitForCompletion();
        }

        if (road.end.Intersection != null) // If there is a junction to choose from
        {
            // Get all roads that lead AWAY from this intersection (no doubling back)
            IEnumerable<RoadAnchor> options = road.end.Intersection.AnchorPoints.Where(x => { x.GetConnectedRoad(out bool isStart); return isStart == true; });
            int numOptions = options.Count();
            if (numOptions > 0)
            {
                int chosenIndex = Random.Range(0, numOptions);
                Road next = options.ElementAt(chosenIndex).GetConnectedRoad();
                StartFollow(next, offset);
            }
            else
            {
                EndOfRoadReached?.Invoke();
            }
        }
    }
    public void StopFollow()
    {
        moveTween.Kill();
        path = new Bezier.OrientedPoint[0];
    }
}
