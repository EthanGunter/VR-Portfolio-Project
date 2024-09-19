using NUnit.Framework;
using SolarStorm.UnityToolkit;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

//[RequireComponent(typeof(Collider))]
public class CardSocket : MonoBehaviour
{
    [SerializeField] Transform _seatedPosition;
    /// <summary>
    /// The distance a card needs to be away from it's socket point before switching views
    /// </summary>
    [Tooltip("The distance, in meters, a card needs to be away from it's socket point before switching views")]
    [SerializeField] FloatRef _transitionDistance = 0.2f;
    /// <summary>
    /// The time it takes for a card to make it back to the socket, in seconds
    /// </summary>
    [Tooltip("The time it takes for a card to make it back to the socket, in seconds")]
    [SerializeField] FloatRef _cardSeatTime = 0.5f;
    [Tooltip("Easing algorithm that determines the card's movement back to the socket when being seated")]
    [SerializeField] EasingFunction _easingFunction = EasingFunction.Linear;

    public event Action<AbilityData> OnAbilityEnterRange;
    public event Action<AbilityData> OnAbilityExitRange;

    public AbilityData AttachedAbility
    {
        get => _abil;
        private set
        {
            if (_abil != null)
            {
                _abil.OnActiveItemGrabbed -= AttachedItemGrabbed;
                _abil.OnActiveItemReleased -= AttachedItemReleased;
            }
            _abil = value;
            if (value != null)
            {
                _cardGrab = _abil.Card.GetComponent<XRGrabInteractable>();
                _previewGrab = _abil.Preview.GetComponent<XRGrabInteractable>();
                name = _abil.name + " socket";
                _abil.OnActiveItemGrabbed += AttachedItemGrabbed;
                _abil.OnActiveItemReleased += AttachedItemReleased;
            }
        }
    }
    private AbilityData _abil;

    private XRGrabInteractable _cardGrab;
    private XRGrabInteractable _previewGrab;
    private float _transitionBuffer = 0.05f; // Prevents excess state change calls on the transition border

    //private AsyncAnimator _animator = new();


    #region Unity Messages

    private void Awake()
    {
        if (_seatedPosition == null)
        {
            _seatedPosition = transform;
        }
    }

    private void FixedUpdate()
    {
        if (AttachedAbility == null) return;

        if (AttachedAbility.State == AbilityState.Card && _cardGrab.GetNewestInteractorSelecting() != null)
        {
            float dist = Vector3.Distance(_seatedPosition.position, AttachedAbility.Card.transform.position);
            if (dist > _transitionDistance + _transitionBuffer)
            {
                // If the card has been pulled far enough away
                AttachedAbility.ChangeState(AbilityState.Preview);
            }
        }
        else if (AttachedAbility.State == AbilityState.Preview && _previewGrab.GetNewestInteractorSelecting() != null)
        {
            float dist = Vector3.Distance(_seatedPosition.position, AttachedAbility.Preview.transform.position);
            if (dist < _transitionDistance - _transitionBuffer)
            {
                // If the card has been returned far enough
                AttachedAbility.ChangeState(AbilityState.Card);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IAbilityComponent abilityHolder))
        {
            if (abilityHolder.Ability == AttachedAbility)
            {
                if (AttachedAbility == null)
                {
                    //TODO card.Ability.OnActiveItemReleased += ForeignCardReleased;
                }
                else
                {
                    Debug.LogWarning("Swapping card locations not yet implemented", this);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IAbilityComponent abilityHolder))
        {
            if (AttachedAbility == null)
            {
                // If a foreign card is leaving the collider zone
                //TODO card.Ability.OnActiveItemReleased += ForeignCardReleased;
            }
        }
    }

    #endregion


    public async void SeatAbility(AbilityData ability, bool skipAnimation = false)
    {
        if (AttachedAbility == null)
        {
            //await _animator.TryCancel(nameof(ReseatCardAnimation));
            AttachedAbility = ability;
        }
        if (ability == null)
        {
            Debug.Log("Seating null ability?", this);
        }

        else if (ability == AttachedAbility)
        {
            //ability.ChangeState(AbilityState.Card);

            // If the animation is already running
            //await _animator.TryCancel(nameof(ReseatCardAnimation));

            //Awaitable animation = null;
            if (!skipAnimation)
            {
                // Start the seating animation
                //animation = _animator.Play(nameof(ReseatCardAnimation), ReseatCardAnimation);
                //await animation;
            }

            AttachedAbility.Card.transform.SetParent(transform);
            AttachedAbility.Card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            AttachedAbility.Card.transform.localScale = Vector3.one;
        }
        else
        {
            //Debug.LogWarning("Card slot switching not implemented");
        }
    }

    public void RemoveActiveAbility()
    {
        //_animator.TryCancel(nameof(ReseatCardAnimation));

        AttachedAbility.Card.transform.SetParent(null);
        AttachedAbility = null;
    }


    #region Event Handlers

    private void AttachedItemGrabbed(AbilityData ability, IXRSelectInteractor interactor)
    {
        //_animator.TryCancel(nameof(ReseatCardAnimation));

        if (ability?.State == AbilityState.Card)
        {
            ability.Card.transform.SetParent(null);
        }
    }
    private void AttachedItemReleased(AbilityData ability, IXRSelectInteractor interactor)
    {
        if (ability?.State == AbilityState.Card)
        {
            SeatAbility(ability);
        }
        else if (_abil?.State == AbilityState.Preview)
        {
            ability.ChangeState(AbilityState.Card);
            SeatAbility(ability);
        }
    }
    private void ForeignCardReleased(AbilityData ability, IXRSelectInteractor interactor)
    {
        AttachedAbility = ability;
        SeatAbility(ability);
    }

    #endregion


    #region Animations

    private async Awaitable ReseatCardAnimation(CancellationToken cancellationToken)
    {
        Vector3 oldPos;
        Quaternion oldRotation;
        switch (AttachedAbility.State)
        {
            case AbilityState.Card:
                oldPos = AttachedAbility.Card.transform.position;
                oldRotation = AttachedAbility.Card.transform.rotation;
                break;
            case AbilityState.Preview:
                oldPos = AttachedAbility.Preview.transform.position;
                oldRotation = AttachedAbility.Preview.transform.rotation;
                break;
            default:
                // If the ability is in any other state, we want to skip the movement animation
                return;
        }

        for (float time = 0; time < _cardSeatTime; time += Time.deltaTime)
        {
            AttachedAbility.Card.transform.SetPositionAndRotation(Vector3.Lerp(oldPos, _seatedPosition.position, EasingFunctions.Ease(time / _cardSeatTime, _easingFunction)), Quaternion.Lerp(oldRotation, _seatedPosition.rotation, EasingFunctions.Ease(time / _cardSeatTime, _easingFunction)));
            AttachedAbility.Preview.transform.SetPositionAndRotation(Vector3.Lerp(oldPos, _seatedPosition.position, EasingFunctions.Ease(time / _cardSeatTime, _easingFunction)), Quaternion.Lerp(oldRotation, _seatedPosition.rotation, EasingFunctions.Ease(time / _cardSeatTime, _easingFunction)));

            if (!cancellationToken.IsCancellationRequested) await Awaitable.EndOfFrameAsync();
            else break;
        }
    }

    #endregion
}