using UnityEngine;
using SolarStorm.UnityToolkit;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))]
public class PlaceAnySurface : AbilityViewComponent, IPlaceable
{
    #region Variables

    [SerializeField] Renderer[] renderers;
    [SerializeField] string colorShaderName = "_Color";

    [SerializeField] LayerMask validSurfaces;
    [SerializeField] GameObject placementPreview;
    [SerializeField] ColorRef validColor;
    [SerializeField] ColorRef invalidColor;
    [SerializeField] FloatRef maxRayLength = 1;

    [Tooltip("If false, surface normal is used")]
    [SerializeField] bool resetRotationOnPlacement;

    private LineRenderer _line;
    private Color _originalColor;

    #endregion


    public override void InitializeAbilityData(AbilityData ability)
    {
        base.InitializeAbilityData(ability);
        Ability.OnActiveItemReleased += Ability_OnActiveItemReleased;
    }


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

        _line = GetComponent<LineRenderer>();
    }
    private void OnDestroy()
    {
        Ability.OnActiveItemReleased -= Ability_OnActiveItemReleased;
    }
    private void Update()
    {
        IsPlacementValid();
    }

    #endregion

    public bool IsPlacementValid()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxRayLength))
        {
            SetColor(_originalColor);
            _line.enabled = false;
            return false;
        }
        else
        {
            //TODO check for overlapping geometry
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

    public bool Place()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, validSurfaces)) return false;

        transform.position = hit.point;
        if (resetRotationOnPlacement)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.Euler(hit.normal);
        }

        ResetVisual();
        return true;
    }

    private void SetColor(Color color)
    {
        if (renderers.Length == 0) return;
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.SetColor(colorShaderName, color);
            }
        }
    }

    private void Ability_OnActiveItemReleased(AbilityData ability, UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
    {
        if (Place())
        {
            Ability.EntityView.transform.SetPositionAndRotation(transform.position, transform.rotation);
            Ability.ChangeState(AbilityState.Active);
        }
        else
        {
            // Put back in hand
            Ability.ChangeState(AbilityState.Card);
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
            //transform.localPosition = position;
            //transform.localRotation = rotation;
        }

        _line.enabled = true;
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, position);
    }
    private void ResetVisual()
    {

        if (placementPreview != null)
        {
            placementPreview.SetActive(false);
        }
        else
        {
            //transform.localPosition = Vector3.zero;
            //transform.localRotation = Quaternion.identity;
        }

        _line.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, validSurfaces)) return;

        Gizmos.DrawRay(hit.point, hit.normal);
    }
}