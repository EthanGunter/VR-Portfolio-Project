using SolarStorm.UnityToolkit;
using System.Threading;
using UnityEngine;


public class DissolvingViewComponent : AbilityViewComponent
{
    #region Variables

    [SerializeField] protected AbilityActivator activator;
    [SerializeField] protected Renderer[] renderers;
    [SerializeField] protected StringRef dissolvePropertyName = "_DissolveAmount";

    [SerializeField] FloatRef _dissolveTime = 0.2f;

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
                }
            }
        }
    }
    private float _dissolvePercent = 1;

    #endregion


    #region Unity Messages

    protected virtual void Awake()
    {
        if (renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
    }

    protected virtual void OnEnable()
    {
        activator.ActivationComplete += OnActivationComplete;
    }
    protected virtual void OnDisable()
    {
        activator.ActivationComplete -= OnActivationComplete;
    }

    #endregion


    public async override Awaitable ShowAsync(CancellationToken cancellationToken)
    {
        Debug.Log("Preview Show - animation begin", this);
        gameObject.SetActive(true);
        while (_dissolvePercent > 0)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log($"Preview Show - stopped early at {_dissolvePercent}", this);
                break;
            }

            _dissolvePercent -= Time.deltaTime / _dissolveTime;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }

        if (!cancellationToken.IsCancellationRequested)
        {
            Debug.Log($"Preview Show - stopped early at {_dissolvePercent}", this);
            _dissolvePercent = 0;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
            }
        }
        Debug.Log("Preview Show - animation completed", this);
    }

    public async override Awaitable HideAsync(CancellationToken cancellationToken)
    {
        Debug.Log("Preview Hide - animation begin", this);
        while (_dissolvePercent < 1)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log($"Preview Hide - stopped early at {_dissolvePercent}", this);
                break;
            }

            _dissolvePercent += Time.deltaTime / _dissolveTime;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }
        if (cancellationToken.IsCancellationRequested)
        {
            Debug.Log($"Preview Hide - stopped early at {_dissolvePercent}", this);
            _dissolvePercent = 1;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat(dissolvePropertyName, _dissolvePercent);
            }
            gameObject.SetActive(false);
        }
        Debug.Log("Preview Hide - animation completed", this);
    }

    public virtual void OnActivationComplete()
    {
        Ability.ChangeState(AbilityState.Active);
    }
}