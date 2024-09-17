using System.Threading;
using UnityEngine;

public class AbilityViewComponent : MonoBehaviour, IView, IAbilityComponent
{
    #region IAbility 

    public AbilityData Ability { get; private set; }
    public void InitializeAbilityData(AbilityData ability)
    {
        Ability = ability;
    }

    #endregion


    #region IView

    /// <summary>
    /// <inheritdoc/>
    /// <para>Note: overrides are not expected to call base()</para>
    /// </summary>
    public virtual void Show()
    {

        Debug.Log($"{name} - Activating", this);
        gameObject.SetActive(true);
    }

    public async virtual Awaitable PlayShowAnimation(CancellationToken cancellationToken) { }

    public async virtual Awaitable PlayHideAnimation(CancellationToken cancellationToken) { }

    /// <summary>
    /// <inheritdoc/>
    /// <para>Note: overrides are not expected to call base()</para>
    /// </summary>
    public virtual void Hide()
    {
        Debug.Log($"{name} - Deactivating", this);
        gameObject.SetActive(false);
    }

    #endregion
}