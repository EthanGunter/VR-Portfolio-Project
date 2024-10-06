using SolarStorm.UnityToolkit;
using UnityEngine;

public abstract class TurretPlacer : AbilityViewComponent, IPlacer
{
    #region Variables

    [SerializeField] Renderer[] renderers;
    [SerializeField] string colorShaderName = "_Color";

    [SerializeField] protected LayerMask validSurfaces;
    [SerializeField] protected GameObject placementPreview;
    [SerializeField] protected ColorRef validColor;
    [SerializeField] protected ColorRef invalidColor;
    [SerializeField] protected FloatRef maxRayLength = 1;

    private LineRenderer _line;
    private Color _originalColor;

    #endregion

    public override void InitializeAbilityData(AbilityData ability)
    {
        base.InitializeAbilityData(ability);
        Ability.OnActiveItemReleased += Ability_OnActiveItemReleased;
    }
    private void OnDestroy()
    {
        Ability.OnActiveItemReleased -= Ability_OnActiveItemReleased;
    }
    private void Update()
    {
        IPlacer.Transform trans = GetValidPlacement();
        if (!trans.Equals(default))
        {
            _line.enabled = true;
            PlaceVisual(trans.position, trans.rotation);
        }
        else
        {
            _line.enabled = false;
        }
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

    #endregion


    public abstract IPlacer.Transform GetValidPlacement();
    public abstract void Place(IPlacer.Transform position);
    protected virtual void SetColor(Color color)
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
    protected virtual void PlaceVisual(Vector3 position, Quaternion rotation)
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
    protected void ResetVisual()
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

    private void Ability_OnActiveItemReleased(AbilityData ability, UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
    {
        IPlacer.Transform trans = GetValidPlacement();
        if (!trans.Equals(default))
        {
            Place(trans);
            Ability.EntityView.transform.SetPositionAndRotation(transform.position, transform.rotation);
            Ability.ChangeState(AbilityState.Active);
            ResetVisual();
        }
        else
        {
            // Put back in hand
            Ability.ChangeState(AbilityState.Card);
        }
    }
}