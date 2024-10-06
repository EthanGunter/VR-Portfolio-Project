using UnityEngine;
using SolarStorm.UnityToolkit;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))]
public class PlaceAnySurface : TurretPlacer
{
    #region Variables

    [SerializeField] bool resetRotationOnPlacement = true;

    #endregion


    #region Unity Messages

    #endregion


    public override IPlacer.Transform GetValidPlacement()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxRayLength, validSurfaces))
        {
            return default;
        }
        else
        {
            //TODO check for overlapping turrets
            if (validSurfaces.Contains(hit.collider.gameObject))
            {
                SetColor(validColor);
                PlaceVisual(hit.point, Quaternion.Euler(hit.normal));
            }
            else
            {
                SetColor(invalidColor);
                PlaceVisual(hit.point, Quaternion.Euler(hit.normal));
            }
            return new IPlacer.Transform { position = hit.point, rotation = Quaternion.Euler(hit.normal) };
        }
    }

    public override void Place(IPlacer.Transform trans)
    {
        transform.position = trans.position;
        if (resetRotationOnPlacement)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = trans.rotation;
        }
    }
}