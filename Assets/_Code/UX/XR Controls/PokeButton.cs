using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[RequireComponent(typeof(XRPokeFilter))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class PokeButton : MonoBehaviour
{
    #region Variables

    [SerializeField] XRBaseInteractable interactable;
    [SerializeField] Transform buttonMoveTransform;
    [SerializeField] Transform endPosition;

    [SerializeField] float hapticIntensity;
    [SerializeField] float positionRecoveryTime = .15f;

    [SerializeField] Renderer activeIndicator;
    [SerializeField] Material active;
    [SerializeField] Material inactive;

    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = value;
            activeIndicator.material = value ? active : inactive;
        }
    }
    private bool isActive;

    public UnityEvent ButtonPressed;


    private XRPokeInteractor poker;
    private XRPokeFilter filter;
    private Vector3 origBtnLocPos;
    private Vector3 locTargPos;
    private Vector3 locMoveDir;
    private float maxDist;

    private IEnumerator anim;
    private bool pressTriggered;

    #endregion


    #region Unity Messages

    private void OnActivated(SelectEnterEventArgs arg0) { ButtonPressed?.Invoke(); }
    private void Awake()
    {
        if (!interactable) throw new MissingReferenceException("Poke Button needs an interactable object to perform events through!");

        interactable.hoverEntered.AddListener(HoverEntered);
        interactable.hoverExited.AddListener(HoverExited);

        origBtnLocPos = transform.InverseTransformPoint(buttonMoveTransform.position);
        locTargPos = transform.InverseTransformPoint(endPosition.position);
        locMoveDir = (locTargPos - origBtnLocPos).normalized;
        maxDist = (endPosition.position - buttonMoveTransform.position).magnitude;

        filter = GetComponent<XRPokeFilter>();
        filter.pokeConfiguration.Value.interactionDepthOffset = -12409821;
        
        
        interactable.firstSelectEntered.AddListener(OnActivated);
    }
    private void OnDestroy()
    {
        interactable.firstSelectEntered.RemoveListener(OnActivated);
    }

    #endregion


    private IEnumerator Move()
    {
        Tween floatTween = null;
        XRPokeInteractor interactor = poker;
        while (poker != null)
        {
            Vector3 projectedLocalPoke = Vector3.Project(transform.InverseTransformPoint(poker.transform.position), locMoveDir);
            float pokeDepth = origBtnLocPos.y - projectedLocalPoke.y;
            float btnDepth = origBtnLocPos.y - buttonMoveTransform.localPosition.y;

            if (pokeDepth < btnDepth)
            {
                if (floatTween == null || !floatTween.active)
                    floatTween = StartReset(poker);
            }
            else if (pokeDepth > 0 && pokeDepth < maxDist)
            {
                DOTween.Kill(this);
                pressTriggered = false;
                buttonMoveTransform.localPosition = projectedLocalPoke;
            }
            else if (pokeDepth >= maxDist)
            {
                buttonMoveTransform.localPosition = locTargPos;
                // TODO trigger select entered? ONCE
                if (!pressTriggered)
                {
                    pressTriggered = true;
                    ButtonPressed?.Invoke();
                    interactable.selectEntered?.Invoke(new SelectEnterEventArgs() { interactableObject = interactable, interactorObject = poker, manager = poker.interactionManager });
                }
            }

            yield return null;
        }

        float visDepth = origBtnLocPos.y - buttonMoveTransform.localPosition.y;

        if (visDepth > 0)
        {
            floatTween = StartReset(interactor);
        }

        while (floatTween != null && floatTween.active) yield return null;
    }

    private Tween StartReset(XRPokeInteractor interactor)
    {
        // Float back up
        pressTriggered = false;
        interactable.selectExited.Invoke(new SelectExitEventArgs() { interactableObject = interactable, interactorObject = interactor, manager = interactor.interactionManager });
        return buttonMoveTransform.DOMove(transform.TransformPoint(origBtnLocPos), positionRecoveryTime).SetId(this);
    }

    private void HoverEntered(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRPokeInteractor poke)
        {
            poker = poke;
            anim = Move();
            StartCoroutine(anim);
        }
    }

    private void HoverExited(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRPokeInteractor poke)
        {
            poker = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // Start
        Gizmos.DrawWireSphere(transform.TransformPoint(origBtnLocPos), .02f);
        Gizmos.color = Color.red; // Stop
        Gizmos.DrawWireSphere(transform.TransformPoint(locTargPos), .02f);

        Gizmos.color = Color.yellow; // Expected move vector
        Vector3 origBtnWorldPos = transform.TransformPoint(origBtnLocPos);
        Gizmos.DrawLine(origBtnWorldPos, origBtnWorldPos + transform.TransformDirection(locMoveDir) * maxDist);

        if (poker)
        {
            Gizmos.color = Color.green; // Current move vector
            Vector3 pokeInLocal = transform.InverseTransformPoint(poker.transform.position);
            Vector3 newPosInLocal = new Vector3(0, pokeInLocal.y, 0); // Locks to "vertical" movement
            Gizmos.DrawLine(transform.position + origBtnLocPos, transform.TransformPoint(newPosInLocal));
        }
    }
}