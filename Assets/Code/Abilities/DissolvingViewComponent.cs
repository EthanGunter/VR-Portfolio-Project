using SolarStorm.UnityToolkit;
using System.Threading;
using UnityEngine;

public class DissolvingViewComponent : AbilityViewComponent
{
    #region Variables

    [SerializeField] protected AbilityActivator activator;
    [SerializeField] protected Renderer[] renderers;
    [SerializeField] protected StringRef dissolvePropertyName = "_DissolveAmount";

    [SerializeField] float _dissolveTime = 0.2f;

    protected float DissolvePercent
    {
        get => _dissolvePercent;
        set
        {
            _dissolvePercent = value;

            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    mat.SetFloat(dissolvePropertyName, _dissolvePercent);
                    mat.SetColor("_Color", Color.red);
                }
            }
        }
    }
    private float _dissolvePercent;

    #endregion


    #region Unity Messages

    protected virtual void OnEnable()
    {
        activator.ActivationComplete += OnActivationComplete;
    }
    protected virtual void OnDisable()
    {
        activator.ActivationComplete -= OnActivationComplete;
    }

    #endregion


    public async override Awaitable PlayShowAnimation(CancellationToken cancellationToken)
    {
        Debug.Log("Preview IN - animation begin", this);
        while (_dissolvePercent > 0)
        {
            if (cancellationToken.IsCancellationRequested) break;

            _dissolvePercent -= Time.deltaTime / _dissolveTime;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 0;
        foreach (var rend in renderers)
        {
            rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
        }
        Debug.Log("Preview IN - animation completed", this);
    }

    public async override Awaitable PlayHideAnimation(CancellationToken cancellationToken)
    {
        Debug.Log("Preview OUT - animation begin", this);
        while (_dissolvePercent < 1)
        {
            if (cancellationToken.IsCancellationRequested) break;

            _dissolvePercent += Time.deltaTime / _dissolveTime;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 1;
        foreach (var rend in renderers)
        {
            rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
        }
        Debug.Log("Preview OUT - animation completed", this);
    }

    public virtual void OnActivationComplete()
    {

        Debug.Log("Ability Activated!", this);
        Ability.ChangeState(AbilityState.Active);
    }

    protected void SetDissolveAmount(float percent)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat(dissolvePropertyName, percent);
        }
    }
}