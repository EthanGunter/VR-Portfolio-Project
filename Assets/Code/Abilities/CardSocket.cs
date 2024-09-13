using SolarStorm.UnityToolkit;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Collider))]
public class CardSocket : MonoBehaviour
{
    [SerializeField] Transform seatedPosition;
    /// <summary>
    /// The distance a card needs to be away from it's socket point before switching views
    /// </summary>
    [Tooltip("The distance, in meters, a card needs to be away from it's socket point before switching views")]
    [SerializeField] FloatRef transitionDistance = 0.2f;
    [SerializeField] float transitionBuffer = 0.01f;
    /// <summary>
    /// The time it takes for a card to make it back to the socket, in seconds
    /// </summary>
    [Tooltip("The time it takes for a card to make it back to the socket, in seconds")]
    [SerializeField] FloatRef _cardSeatTime = 0.5f;
    [Tooltip("Easing algorithm that determines the card's movement back to the socket when being seated")]
    [SerializeField] EasingFunction easingFunction = EasingFunction.Linear;

    public AbilityData AttachedAbility
    {
        get => abil;
        private set
        {
            if (abil != null)
            {
                abil.OnActiveItemGrabbed -= AttachedItemGrabbed;
                abil.OnActiveItemReleased -= AttachedItemReleased;
            }
            abil = value;
            if (value != null)
            {
                abil.OnActiveItemGrabbed += AttachedItemGrabbed;
                abil.OnActiveItemReleased += AttachedItemReleased;
            }
        }
    }
    private AbilityData abil;

    // Animation cancellation vars
    private Awaitable _seatAnim;
    private CancellationTokenSource _seatAnimCTS;


    #region Unity Messages

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        if (seatedPosition == null)
        {
            seatedPosition = transform;
        }
    }

    private void FixedUpdate()
    {
        if (abil.State == AbilityState.Card && abil.Card.HeldBy != null)
        {
            // If card gets pulled away from socket
            float cardDistFromSocket = Vector3.Distance(seatedPosition.position, AttachedAbility.Card.transform.position);
            if (cardDistFromSocket > transitionDistance + transitionBuffer)
            {
                AttachedAbility.ChangeState(AbilityState.Preview);
            }
        }
        else if (abil.State == AbilityState.Preview && abil.Preview.HeldBy != null)
        {
            // If preview gets returned to socket
            float previewDistFromSocket = Vector3.Distance(seatedPosition.position, AttachedAbility.Preview.transform.position);
            if (previewDistFromSocket < transitionDistance - transitionBuffer)
            {
                AttachedAbility.ChangeState(AbilityState.Card);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Card card))
        {
            if (AttachedAbility == null)
            {
                //card.OnReleased += ForeignCardReleased;
            }
            else if (card != AttachedAbility.Card)
            {
                Debug.LogWarning("Swapping card locations not yet implemented");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Card card))
        {
            // If a foreign card is leaving the collider zone
            if (card != AttachedAbility.Card)
            {
                //card.OnReleased += ForeignCardReleased;
            }
        }
    }

    #endregion


    public async void SeatAbility(AbilityData ability, bool skipAnimation = false)
    {
        if (AttachedAbility == null)
        {
            AttachedAbility = ability;
        }

        //AttachedAbility.Card.OnGrabbed += AttachedItemGrabbed;
        //AttachedAbility.Card.OnReleased += AttachedItemReleased;

        if (ability == AttachedAbility)
        {
            ability.ChangeState(AbilityState.Card);

            // If the animation is already running
            if (_seatAnim != null && !_seatAnim.IsCompleted)
            {
                // Cancel it
                _seatAnimCTS.Cancel();

                // Wait for it to wrap up
                await _seatAnim;
            }

            if (!skipAnimation)
            {
                // Start the seating animation
                _seatAnimCTS = new CancellationTokenSource();
                _seatAnim = SeatCardAnimation(_seatAnimCTS.Token);
                await _seatAnim;
                _seatAnim = null;
            }

            // If the animation isn't cancelled by the time we get here, assign final variables
            if (_seatAnim == null || !_seatAnimCTS.Token.IsCancellationRequested)
            {
                AttachedAbility.Card.transform.SetParent(transform);
                AttachedAbility.Card.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                AttachedAbility.Card.transform.localScale = Vector3.one;
            }
        }
        else
        {
            //Debug.LogWarning("Card slot switching not implemented");
        }
    }

    public void RemoveActiveAbility()
    {
        if (_seatAnim != null)
        {
            _seatAnimCTS.Cancel();
        }

        //AttachedAbility.Card.OnGrabbed -= AttachedItemGrabbed;
        //AttachedAbility.Card.OnReleased -= AttachedItemReleased;

        AttachedAbility.Card.transform.SetParent(null);
        AttachedAbility = null;
    }


    #region Event Handlers

    private void AttachedItemGrabbed(AbilityData ability, IXRSelectInteractor interactor)
    {
        if (_seatAnim != null)
        {
            _seatAnimCTS.Cancel();
        }

        if (ability.State == AbilityState.Card)
        {
            ability.Card.transform.SetParent(null);
        }
    }
    private void AttachedItemReleased(AbilityData ability, IXRSelectInteractor interactor)
    {
        if (ability.State == AbilityState.Card)
        {
            SeatAbility(ability);
        }
        else if (abil.State == AbilityState.Preview)
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

    private async Awaitable SeatCardAnimation(CancellationToken cancellationToken)
    {
        Vector3 oldPos = AttachedAbility.Card.transform.position;
        Quaternion oldRotation = AttachedAbility.Card.transform.rotation;

        for (float time = 0; time < _cardSeatTime; time += Time.deltaTime)
        {
            AttachedAbility.Card.transform.SetPositionAndRotation(Vector3.Lerp(oldPos, seatedPosition.position, EasingFunctions.Ease(time / _cardSeatTime, easingFunction)), Quaternion.Lerp(oldRotation, seatedPosition.rotation, EasingFunctions.Ease(time / _cardSeatTime, easingFunction)));

            if (!cancellationToken.IsCancellationRequested) await Awaitable.EndOfFrameAsync();
            else break;
        }
    }

    #endregion
}