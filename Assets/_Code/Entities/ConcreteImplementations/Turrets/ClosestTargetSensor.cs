using SolarStorm.Entities;
using SolarStorm.UnityToolkit;
using UnityEngine;

public class ClosestTargetSensor : MonoBehaviour, ITargetSensor
{
    #region Variables

    [SerializeField] GameObjectRuntimeSet targetSet;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] FloatRef maxDistance;

    #endregion


    #region Unity Messages

    #endregion

    public HealthComponent GetTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance, targetLayers);
        HealthComponent closestTarget = null;
        float closestDist = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out HealthComponent potentialTarget))
            {
                if (!potentialTarget.IsAlive) continue;

                float dist = Vector3.Distance(potentialTarget.transform.position, transform.position);
                if (dist < closestDist)
                {
                    closestTarget = potentialTarget;
                    closestDist = dist;
                }
            }
        }

        return closestTarget;
    }
}