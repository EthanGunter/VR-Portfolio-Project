using SolarStorm.Entities;
using SolarStorm.UnityToolkit;
using UnityEngine;

public class ClosestTargetSensor : MonoBehaviour, ITargetSensor
{
    #region Variables

    [SerializeField] GameObjectSet targetSet;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] FloatRef maxDistance;

    #endregion


    #region Unity Messages

    #endregion

    public GameObject GetTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance, targetLayers);
        GameObject closestTarget = null;
        float closestDist = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out HealthComponent potentialTarget))
            {
                float dist = Vector3.Distance(potentialTarget.transform.position, transform.position);
                if (dist < closestDist)
                {
                    closestTarget = potentialTarget.gameObject;
                    closestDist = dist;
                }
            }
        }

        return closestTarget;
    }
}