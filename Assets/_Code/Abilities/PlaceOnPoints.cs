using SolarStorm.UnityToolkit;
using UnityEngine;

public class PlaceOnPoints : TurretPlacer
{
    [Tooltip("How far from a point does a placer need to be before it registers as a valid placement?")]
    [SerializeField] float sphereCastRadius = 1;
    [SerializeField] bool keepTurretYRotation = true;

    private PlacementPoint currentPoint;

    public override IPlacer.Transform GetValidPlacement()
    {
        if (Physics.SphereCast(transform.position, sphereCastRadius, Vector3.down, out RaycastHit hit, maxRayLength, validSurfaces))
        {
            if (hit.collider.TryGetComponent(out PlacementPoint point))
            {
                return new IPlacer.Transform { position = hit.point, rotation = Quaternion.Euler(hit.normal) };
            }
        }

        return default;
    }

    public override void Place(IPlacer.Transform position)
    {
        if (currentPoint != null)
        {
            currentPoint.Place(gameObject, keepTurretYRotation);
        }
    }
}