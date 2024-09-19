using System.Threading;
using UnityEngine;

/* Note for future self:
 * Card and Preview components are logical views.
 * It's ok for these view components to implement game-changing logic.
 * Their whole goal is to present visuals and switch from card to entity and that's it.
 */
public class AbilityViewComponent : MonoBehaviour, IView, IAbilityComponent
{
    #region IAbility 

    public AbilityData Ability { get; private set; }
    public virtual void InitializeAbilityData(AbilityData ability)
    {
        Ability = ability;
        IAbilityComponent[] abComps = GetComponents<IAbilityComponent>();
        if (abComps.Length > 0)
        {
            foreach (var abComp in abComps)
            {
                if(abComp == this) continue;
                abComp.InitializeAbilityData(ability);
            }
        }
    }

    #endregion


    #region IView

    /// <summary>
    /// <inheritdoc/>
    /// <para>Note: overrides are not expected to call base()</para>
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public async virtual Awaitable ShowAsync(CancellationToken cancellationToken) { }

    public async virtual Awaitable HideAsync(CancellationToken cancellationToken) { }

    /// <summary>
    /// <inheritdoc/>
    /// <para>Note: overrides are not expected to call base()</para>
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion
}