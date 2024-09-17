using UnityEngine;
using SolarStorm.UnityToolkit;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))]
public class PlaceAnySurface : MonoBehaviour, IPlaceable
{
    #region Variables

    [SerializeField] Renderer[] renderers;
    [SerializeField] string colorShaderName = "_Color";

    [SerializeField] LayerMask validSurfaces;
    [SerializeField] GameObject placementPreview;
    [SerializeField] ColorRef validColor;
    [SerializeField] ColorRef invalidColor;

    [Tooltip("If false, surface normal is used")]
    [SerializeField] bool resetRotationOnPlacement;

    private LineRenderer _line;
    private Color _originalColor;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        if (renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        if (renderers.Length > 0)
        {
            _originalColor = renderers[0].material.GetColor(colorShaderName);
        }

        if (placementPreview != null)
        {
            // Overwrite the prefab with an instance object
            placementPreview = Instantiate(placementPreview, transform);
        }
    }

    #endregion


    public bool IsPlacementValid()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue);
        if (hit.collider == null)
        {
            SetColor(_originalColor);
            return false;
        }
        else
        {
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
            return true;
        }
    }

    public void Place()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, validSurfaces);
        transform.position = hit.point;
        if (resetRotationOnPlacement)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.Euler(hit.normal);
        }
    }

    private void SetColor(Color color)
    {
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.SetColor(colorShaderName, color);
            }
        }
    }

    private void PlaceVisual(Vector3 position, Quaternion rotation)
    {
        if (placementPreview != null)
        {
            placementPreview.SetActive(true);
            placementPreview.transform.position = position;
            placementPreview.transform.rotation = rotation;
        }
        else
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
    }
    private void ResetVisual()
    {

        if (placementPreview != null)
        {
            placementPreview.SetActive(false);
        }
        else
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}