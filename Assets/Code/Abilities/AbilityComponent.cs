using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public abstract class AbilityComponent : MonoBehaviour, IView
{
    #region Variables

    //public event AbilitySelectEvent OnGrabbed;
    //public event AbilitySelectEvent OnReleased;

    public AbilityData Ability { get; private set; }
    public IXRSelectInteractor HeldBy { get; private set; }

    public XRGrabInteractable Grabbable { get; private set; }

    #endregion

    protected virtual void Awake()
    {
        Grabbable = GetComponent<XRGrabInteractable>();
        Grabbable.firstSelectEntered.AddListener(Grabbed);
        Grabbable.lastSelectExited.AddListener(Released);
    }

    public void Initialize(AbilityData ability)
    {
        Ability = ability;
    }

    public void AttachToHand(IXRSelectInteractor selector)
    {
        if (selector != null)
        {
            Grabbable.interactionManager.SelectEnter(selector, Grabbable);
        }
    }


    #region Event Handlers

    private void Grabbed(SelectEnterEventArgs args)
    {
        HeldBy = args.interactorObject;
        //OnGrabbed?.Invoke(Ability, args.interactorObject);
    }
    private void Released(SelectExitEventArgs args)
    {
        //OnReleased?.Invoke(Ability, args.interactorObject);
        HeldBy = null;
    }

    #endregion

    public abstract Awaitable Hide(bool skipAnimation = false);
    public abstract Awaitable Show(bool skipAnimation = false);
    //protected abstract Awaitable Hide_internal();
    //protected abstract Awaitable Show_internal();
}