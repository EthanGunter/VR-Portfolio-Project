using SolarStorm.Entities;
using SolarStorm.UnityToolkit;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetInRangeSensor : EntityComponent, ITargetSensor
{
    [Header("Targeting")]
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] FloatRef maxTargetingDistance;

    public GameEntity Target { get; private set; }

    public event Action<GameEntity> OnTargetAcquired;
    public event Action<GameEntity> OnTargetLost;

    private void AcquireTargets()
    {
        // If the target goes out of range, unset it
        if (Target != null)
        {
            if (Vector3.Distance(Target.transform.position, transform.position) > maxTargetingDistance)
            {
                OnTargetLost?.Invoke(Target);
                Target = null;
            }
        }

        // Then, if there is no target, get the closest one and set it as the target
        if (Target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, maxTargetingDistance, targetLayers);
            GameEntity closestTarget = null;
            float closestDist = float.MaxValue;
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out GameEntity potentialTarget))
                {
                    float dist = Vector3.Distance(potentialTarget.transform.position, transform.position);
                    if (dist < closestDist)
                    {
                        closestTarget = potentialTarget;
                        closestDist = dist;
                    }
                }
            }

            Target = closestTarget;
            OnTargetAcquired?.Invoke(Target);
        }
    }
}