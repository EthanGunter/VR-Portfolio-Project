using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Card : AbilityComponent
{
    #region Variables

    [SerializeField] Renderer _rend;
    [SerializeField] float _dissolveTime;

    private float _dissolvePercent;

    #endregion


    #region Unity Messages

    #endregion


    public async override Awaitable Show(bool skipAnimation = false)
    {
        gameObject.SetActive(true);
        //await Show_internal();
    }

    public async override Awaitable Hide(bool skipAnimation = false)
    {
        //await Hide_internal();
        gameObject.SetActive(false);
    }


    // TODO These should take cancellation tokens, and show and hide should cancel whatever the active task is...
    protected async /*override*/ Awaitable Show_internal()
    {
        while (_dissolvePercent > 0)
        {
            _dissolvePercent -= Time.deltaTime / _dissolveTime;
            _rend.material.SetFloat("_DissolveAmount", _dissolvePercent);
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 0;
    }
    protected async /*override*/ Awaitable Hide_internal()
    {
        while (_dissolvePercent < 1)
        {
            _dissolvePercent += Time.deltaTime / _dissolveTime;
            _rend.material.SetFloat("_DissolveAmount", _dissolvePercent);
            await Awaitable.EndOfFrameAsync();
        }
        _dissolvePercent = 1;
    }
}