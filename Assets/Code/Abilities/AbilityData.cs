using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public delegate void AbilitySelectEvent(AbilityData ability, IXRSelectInteractor interactor);

[CreateAssetMenu(menuName = "Abilities/Ability")]
public class AbilityData : ScriptableObject
{
    string id = Path.GetRandomFileName().Substring(0, 4);

    #region Compile-time Inspector fields

    [field: SerializeField] public string AbilityName { get; private set; }
    [SerializeField] private Card _cardPrefab;
    [SerializeField] private AbilityViewComponent _previewPrefab;
    [SerializeField] private AbilityViewComponent _entityPrefab;

    #endregion


    #region Runtime Variables

    public event Action<AbilityData> OnStateChangeBegin;
    public event Action<AbilityData> OnStateChange;
    public event AbilitySelectEvent OnActiveItemGrabbed;
    public event AbilitySelectEvent OnActiveItemReleased;

    public Card Card
    {
        get
        {
            if (_card == null)
            {
                _card = Instantiate(_cardPrefab);
                _card.InitializeAbilityData(this);
                _card.gameObject.SetActive(false);
                _card.name = _card.Text;
            }
            return _card;
        }
        private set { _card = value; }
    }
    private Card _card;
    public AbilityViewComponent Preview
    {
        get
        {
            if (_preview == null)
            {
                _preview = Instantiate(_previewPrefab);
                _preview.InitializeAbilityData(this);
                _preview.gameObject.SetActive(false);
                _preview.name = Card.Text;
            }
            return _preview;
        }
        private set { _preview = value; }
    }
    private AbilityViewComponent _preview;
    public AbilityViewComponent EntityView
    {
        get
        {
            if (_entity == null)
            {
                _entity = Instantiate(_entityPrefab);
                _entity.InitializeAbilityData(this);
                _entity.gameObject.SetActive(false);
                _entity.name = Card.Text;
            }
            return _entity;
        }
        private set { _entity = value; }
    }
    private AbilityViewComponent _entity;

    public AbilityState State
    {
        get => _state;
        private set
        {
            if (_state != value)
            {
                _state = value;
            }
        }
    }
    private AbilityState _state;

    //private AsyncAnimator _animator = new();

    #endregion

    /// <summary>
    /// Changes the ability to one of <see cref="AbilityState"/>, and (should) performs an awaitable transition animation
    /// </summary>
    // Card <-> Preview -> Active ?-> Card
    public async Awaitable ChangeState(AbilityState newState, IXRSelectInteractor forceToHand = null)
    {
        try
        {
            if (State == newState) return;

            //Debug.Log($"{id} changing: {_state}->{newState}", this);

            try { OnStateChangeBegin?.Invoke(this); } catch (Exception e) { Debug.LogException(e); }

            AbilityState oldState = State;
            State = newState;

            if (newState == AbilityState.Card && oldState == AbilityState.Consumed)
            {
                // Card birth
                XRGrabInteractable cardGrab = Card.GetComponent<XRGrabInteractable>();
                cardGrab.firstSelectEntered.AddListener(ForwardItemGrabbedEvent);
                cardGrab.lastSelectExited.AddListener(ForwardItemReleasedEvent);

                Card.Show();
            }
            else if (newState == AbilityState.Card && oldState == AbilityState.Preview)
            {
                // Preview -> Card

                XRGrabInteractable previewGrab = Preview.GetComponent<XRGrabInteractable>();
                XRGrabInteractable cardGrab = Card.GetComponent<XRGrabInteractable>();

                // Stop listening to the preview's grab/release events
                previewGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
                previewGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);

                // Force the card into the player's hand
                if (forceToHand != null) cardGrab.interactionManager.SelectEnter(forceToHand, cardGrab);
                else if (previewGrab.GetNewestInteractorSelecting() != null) cardGrab.interactionManager.SelectEnter(previewGrab.GetNewestInteractorSelecting(), cardGrab);
                else // The card needs to go back home
                {
                    cardGrab.lastSelectExited.Invoke(new SelectExitEventArgs() { interactableObject = cardGrab });
                }

                // Then listen for its grab/release events
                cardGrab.firstSelectEntered.AddListener(ForwardItemGrabbedEvent);
                cardGrab.lastSelectExited.AddListener(ForwardItemReleasedEvent);

                // TODO
                // Run the actual animations
                Card.Show();
                Preview.Hide();
            }
            else if (newState == AbilityState.Preview)
            {
                // Card -> Preview
                if (oldState != AbilityState.Card) { throw new InvalidOperationException($"{AbilityName} attempted to enter Preview state from {newState}. Should be in Card state."); }

                XRGrabInteractable previewGrab = Preview.GetComponent<XRGrabInteractable>();
                XRGrabInteractable cardGrab = Card.GetComponent<XRGrabInteractable>();

                // Stop listening to the cards's grab/release events
                cardGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
                cardGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);

                // Force the preview into the player's hand
                if (forceToHand != null) previewGrab.interactionManager.SelectEnter(forceToHand, previewGrab);
                else if (cardGrab.GetNewestInteractorSelecting() != null) previewGrab.interactionManager.SelectEnter(cardGrab.GetNewestInteractorSelecting(), previewGrab);

                // Then listen for its grab/release events
                previewGrab.firstSelectEntered.AddListener(ForwardItemGrabbedEvent);
                previewGrab.lastSelectExited.AddListener(ForwardItemReleasedEvent);

                // TODO
                // Run the actual animations
                Preview.Show();
                Card.Hide();
            }
            else if (newState == AbilityState.Active)
            {
                // Preview -> Active
                if (oldState != AbilityState.Preview) { throw new InvalidOperationException($"{AbilityName} attempted to enter Active state from {newState}. Should be in Preview state."); }

                XRGrabInteractable previewGrab = Preview.GetComponent<XRGrabInteractable>();

                // Stop listening to the preview's grab/release events
                previewGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
                previewGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);

                // If the active item is grabbable
                if (EntityView.TryGetComponent(out XRGrabInteractable activeGrab))
                {
                    // Force it into the player's hand
                    if (forceToHand != null) activeGrab.interactionManager.SelectEnter(forceToHand, activeGrab);
                    else if (previewGrab.GetNewestInteractorSelecting() != null) activeGrab.interactionManager.SelectEnter(previewGrab.GetNewestInteractorSelecting(), activeGrab);

                    // Then listen for its grab/release events
                    activeGrab.firstSelectEntered.AddListener(ForwardItemGrabbedEvent);
                    activeGrab.lastSelectExited.AddListener(ForwardItemReleasedEvent);
                }

                // TODO
                // Run the actual animations
                EntityView.Show();
                Destroy(Preview.gameObject);
                //Destroy(Card.gameObject);
            }
            else if (newState == AbilityState.Consumed)
            {
                // Active -> Consumed
                if (oldState != AbilityState.Active) { throw new InvalidOperationException($"{AbilityName} attempted to enter consumed state from {newState}. Should be in Active state."); }

                if (EntityView.TryGetComponent(out XRGrabInteractable activeGrab))
                {
                    // If the active item is grabbable
                    // Stop listening to its grab/release events
                    activeGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
                    activeGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);
                }

                EntityView.Hide();
                Destroy(EntityView.gameObject);
            }

            //Debug.Log($"{id} changed: {oldState}->{newState}", this);

            try { OnStateChange?.Invoke(this); } catch (Exception e) { Debug.LogException(e); }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    private void ForwardItemGrabbedEvent(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out IAbilityComponent ac))
        {
            OnActiveItemGrabbed?.Invoke(ac.Ability, args.interactorObject);
        }
        else
        {
            throw new InvalidOperationException($"{args.interactableObject.transform.name} has no AbilityComponent on it. Cannot forward Grabbed event");
        }
    }
    private void ForwardItemReleasedEvent(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out IAbilityComponent ac))
        {
            OnActiveItemReleased?.Invoke(ac.Ability, args.interactorObject);
        }
        else
        {
            throw new InvalidOperationException($"{args.interactableObject.transform.name} has no AbilityComponent on it. Cannot forward Released event");
        }
    }
}