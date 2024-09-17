using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using SolarStorm.UnityToolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Card : AbilityViewComponent
{
    #region Variables

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Renderer[] _renderers;
    [Header("Dissolve FX")]
    [SerializeField] StringRef _dissolvePropertyName = "_DissolveAmount";
    [SerializeField] FloatRef _dissolveTime = .2f;
    [Header("Hover color FX")]
    [SerializeField] StringRef _emissionColorPropertyName = "_EmissionColor";
    [SerializeField][ColorUsage(false, true)] Color _hoverColor;
    [SerializeField][ColorUsage(false, true)] Color _selectColor;
    [SerializeField][ColorUsage(false, true)] Color _activateColor;
    [SerializeField][ColorUsage(false, true)] Color _focusColor;

    [field: SerializeField] public string Text { get; private set; } // TODO look into pulling this data from a scriptable spreadsheet

    protected float DissolvePercent
    {
        get => _dissolvePercent;
        set
        {
            _dissolvePercent = value;

            foreach (var renderer in _renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    mat.SetFloat(_dissolvePropertyName, _dissolvePercent);
                }
            }
        }
    }
    private float _dissolvePercent;

    private XRGrabInteractable _grab;
    private Color _initColor;

    #endregion


    #region Unity Messages

    protected virtual void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        _initColor = _renderers[0].material.GetColor(_emissionColorPropertyName);
    }
    protected virtual void Start()
    {
        _text.text = Text;
    }

    protected virtual void OnEnable()
    {
        _grab.firstHoverEntered.AddListener(HoverEnter);
        _grab.lastHoverExited.AddListener(HoverExit);
        _grab.firstSelectEntered.AddListener(Select);
        _grab.lastSelectExited.AddListener(SelectExit);
    }
    protected virtual void OnDisable()
    {
        _grab.firstHoverEntered.RemoveListener(HoverEnter);
        _grab.lastHoverExited.RemoveListener(HoverExit);
        _grab.firstSelectEntered.RemoveListener(Select);
        _grab.lastSelectExited.RemoveListener(SelectExit);
    }

    #endregion


    #region Event Handlers

    public void HoverEnter(HoverEnterEventArgs args)
    {
        SetEmissionColor(_hoverColor);
    }
    public void HoverExit(HoverExitEventArgs args)
    {
        if (!_grab.isSelected)
        {
            SetEmissionColor(_initColor);
        }
    }
    public void Select(SelectEnterEventArgs args)
    {
        SetEmissionColor(_selectColor);
    }
    public void SelectExit(SelectExitEventArgs args)
    {
        SetEmissionColor(_initColor);
    }

    #endregion



    public async override Awaitable PlayShowAnimation(CancellationToken cancellationToken)
    {
        Debug.Log("Card IN - animation begin", this);
        while (_dissolvePercent > 0)
        {
            if (cancellationToken.IsCancellationRequested) break;

            _dissolvePercent -= Time.deltaTime / _dissolveTime;
            foreach (var rend in _renderers)
            {
                rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 0;
        foreach (var rend in _renderers)
        {
            rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
        }
        Debug.Log("Card IN - animation complete", this);
    }

    public async override Awaitable PlayHideAnimation(CancellationToken cancellationToken)
    {
        Debug.Log("Card OUT - animation begin", this);
        while (_dissolvePercent < 1)
        {
            if (cancellationToken.IsCancellationRequested) break;

            _dissolvePercent += Time.deltaTime / _dissolveTime;
            foreach (var rend in _renderers)
            {
                rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 1;
        foreach (var rend in _renderers)
        {
            rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
        }
        Debug.Log("Card OUT - animation complete", this);
    }


    #region Utility

    private void SetEmissionColor(Color color)
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetColor(_emissionColorPropertyName, color);
        }
    }

    #endregion
}