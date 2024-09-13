using UnityEngine;

public class Preview : AbilityComponent
{
    #region Variables

    [SerializeField] AbilityActivator activator;
    [SerializeField] Renderer[] renderers;

    [SerializeField] float _dissolveTime = 0.2f;

    private float _dissolvePercent;

    #endregion


    #region Unity Messages

    #endregion


    public async override Awaitable Show(bool skipAnimation = false)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_DissolveAmount", 0);
        }
        gameObject.SetActive(true);
        //await Show_internal();
    }

    public async override Awaitable Hide(bool skipAnimation = false)
    {
        //await Hide_internal();
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_DissolveAmount", 1);
        }
        gameObject.SetActive(false);
    }

    public virtual void ActivateCard()
    {
        Ability.ChangeState(AbilityState.Active);
    }

    public void SetDissolveAmount(float percent)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_DissolveAmount", percent);
        }
    }

    // TODO These should take cancellation tokens, and show and hide should cancel whatever the active task is...
    protected async /*override*/ Awaitable Show_internal()
    {
        while (_dissolvePercent > 0)
        {
            _dissolvePercent -= Time.deltaTime / _dissolveTime;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat("_DissolveAmount", _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 0;
        foreach (var rend in renderers)
        {
            rend.material.SetFloat("_DissolveAmount", _dissolvePercent);
        }
    }
    protected async /*override*/ Awaitable Hide_internal()
    {
        while (_dissolvePercent < 1)
        {
            _dissolvePercent += Time.deltaTime / _dissolveTime;
            foreach (var rend in renderers)
            {
                rend.material.SetFloat("_DissolveAmount", _dissolvePercent);
            }
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 1;
        foreach (var rend in renderers)
        {
            rend.material.SetFloat("_DissolveAmount", _dissolvePercent);
        }
    }
}