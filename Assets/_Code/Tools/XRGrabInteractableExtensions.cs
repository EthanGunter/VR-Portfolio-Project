using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public static class XRGrabInteractableExtensions
{
    public static IXRSelectInteractor GetNewestInteractorSelecting(this XRGrabInteractable grab)
    {
        return grab.interactorsSelecting.Count > 0 ? grab.interactorsSelecting[grab.interactorsSelecting.Count - 1] : null;
    }

}