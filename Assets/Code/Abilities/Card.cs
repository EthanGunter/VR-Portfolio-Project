using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using SolarStorm.UnityToolkit;
using System.IO;

[RequireComponent(typeof(XRGrabInteractable))]
public class Card : AbilityViewComponent
{
    #region Variables

    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] TextMeshProUGUI _activeText;
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
    private float _dissolvePercent = 1;

    private XRGrabInteractable _grab;
    private Color _initColor;

    #endregion

    string _rand = Path.GetRandomFileName().Substring(0, 2); // TEMP
    // "Constructor"
    public override void InitializeAbilityData(AbilityData ability)
    {
        base.InitializeAbilityData(ability);
        Ability.OnStateChange += Ability_OnStateChangeBegin;
        name = Ability.AbilityName + " " + _rand;
        _nameText.text = name;
    }


    #region Unity Messages

    protected virtual void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        _initColor = _renderers[0].material.GetColor(_emissionColorPropertyName);
        _activeText.gameObject.SetActive(false);
        _text.gameObject.SetActive(false);
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
    private void OnDestroy()
    {
        Ability.OnStateChange -= Ability_OnStateChangeBegin;
    }

    #endregion


    #region Event Handlers

    private void HoverEnter(HoverEnterEventArgs args)
    {
        SetEmissionColor(_hoverColor);
    }
    private void HoverExit(HoverExitEventArgs args)
    {
        if (!_grab.isSelected)
        {
            SetEmissionColor(_initColor);
        }
    }
    private void Select(SelectEnterEventArgs args)
    {
        SetEmissionColor(_selectColor);
    }
    private void SelectExit(SelectExitEventArgs args)
    {
        SetEmissionColor(_initColor);
    }

    private void Ability_OnStateChangeBegin(AbilityData obj)
    {
        _activeText.gameObject.SetActive(true);
        _activeText.text = Ability.State.ToString();
        //if (Ability.State == AbilityState.Active)
        //{
        //    _activeText.gameObject.SetActive(true);
        //}
        //else
        //{
        //    _activeText.gameObject.SetActive(false);
        //}
    }

    #endregion


    public async override Awaitable PlayShowAnimation(CancellationToken cancellationToken)
    {
        //Debug.Log("Card Show - animation begin", this);
        while (_dissolvePercent > 0 && !cancellationToken.IsCancellationRequested)
        {
            _dissolvePercent -= Time.deltaTime / _dissolveTime;
            foreach (var rend in _renderers)
            {
                rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }

        if (!cancellationToken.IsCancellationRequested)
        {
            _dissolvePercent = 0;
            foreach (var rend in _renderers)
            {
                rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
            }
        }
        //Debug.Log("Card Show - animation complete", this);
    }

    public async override Awaitable PlayHideAnimation(CancellationToken cancellationToken)
    {
        //Debug.Log("Card Hide - animation begin", this);
        while (_dissolvePercent < 1 && !cancellationToken.IsCancellationRequested)
        {

            _dissolvePercent += Time.deltaTime / _dissolveTime;
            foreach (var rend in _renderers)
            {
                rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }

        if (!cancellationToken.IsCancellationRequested)
        {
            _dissolvePercent = 1;
            foreach (var rend in _renderers)
            {
                rend.material.SetFloat(_dissolvePropertyName, _dissolvePercent);
            }
        }
        //Debug.Log("Card Hide - animation complete", this);
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