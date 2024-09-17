using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public delegate void AbilitySelectEvent(AbilityData ability, IXRSelectInteractor interactor);

[CreateAssetMenu(menuName = "Abilities/Ability")]
public class AbilityData : ScriptableObject
{
    #region Compile-time Inspector fields

    [SerializeField] private Card _cardPrefab;
    [SerializeField] private AbilityViewComponent _previewPrefab;
    [SerializeField] private AbilityViewComponent _entityPrefab;

    #endregion


    #region Runtime Variables

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
            }
            return _card;
        }
        private set { _card = value; }
    }
    private Card _card;
    private CancellationTokenSource _cardAnimation;
    public AbilityViewComponent Preview
    {
        get
        {
            if (_preview == null)
            {
                _preview = Instantiate(_previewPrefab);
                _preview.InitializeAbilityData(this);
            }
            return _preview;
        }
        private set { _preview = value; }
    }
    private AbilityViewComponent _preview;
    private CancellationTokenSource _previewAnimation;
    public AbilityViewComponent EntityView
    {
        get
        {
            if (_entity == null)
            {
                _entity = Instantiate(_entityPrefab);
            }
            return _entity;
        }
        private set { _entity = value; }
    }
    private AbilityViewComponent _entity;
    private CancellationTokenSource _entityViewAnimation;

    public AbilityState State
    {
        get => _state;
        private set
        {
            if (_state != value)
            {
                _state = value;
                OnStateChange?.Invoke(this);
            }
        }
    }
    private AbilityState _state = AbilityState.Card;

    #endregion


    // Card <-> Preview -> Active ?-> Card
    public async Awaitable ChangeState(AbilityState state)
    {
        if (State == state)
        {
            return;
        }

        State = state;

        if (state == AbilityState.Card) // Could be transitioning from Preview or Consumed state
        {            
            // TODO Check if we're coming from consumed state...

            XRGrabInteractable previewGrab = Preview.GetComponent<XRGrabInteractable>();
            XRGrabInteractable cardGrab = Card.GetComponent<XRGrabInteractable>();

            // Stop listening to the preview's grab/release events
            previewGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
            previewGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);
            Debug.Log("Stopped listening to card grab/release", cardGrab);

            // Force the card into the player's hand, and listen for its grab/release events

            cardGrab.interactionManager.SelectEnter(previewGrab.GetNewestInteractorSelecting(), cardGrab);
            cardGrab.firstSelectEntered.AddListener(ForwardItemGrabbedEvent);
            cardGrab.lastSelectExited.AddListener(ForwardItemReleasedEvent);
            Debug.Log("Listening to card grab/release", cardGrab);


            Awaitable hidePreviewAnim = Preview.PlayHideAnimation(RefreshAnimationToken(ref _previewAnimation));
            await hidePreviewAnim;
            Preview.Hide();


            Card.Show();
            await Card.PlayShowAnimation(RefreshAnimationToken(ref _cardAnimation));
        }
        else if (state == AbilityState.Preview) // Can only transition to Preview from Card
        {
            XRGrabInteractable previewGrab = Preview.GetComponent<XRGrabInteractable>();
            XRGrabInteractable cardGrab = Card.GetComponent<XRGrabInteractable>();

            // Stop listening to the cards's grab/release events
            cardGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
            cardGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);
            Debug.Log("Stopped listening to card grab/release", cardGrab);

            // Force the preview into the player's hand, and listen for its grab/release events
            previewGrab.interactionManager.SelectEnter(cardGrab.GetNewestInteractorSelecting(), previewGrab);
            previewGrab.firstSelectEntered.AddListener(ForwardItemGrabbedEvent);
            previewGrab.lastSelectExited.AddListener(ForwardItemReleasedEvent);
            Debug.Log("Listening to preview grab/release", cardGrab);

            await Card.PlayHideAnimation(RefreshAnimationToken(ref _cardAnimation));
            Card.Hide();

            Preview.Show();
            await Preview.PlayShowAnimation(RefreshAnimationToken(ref _previewAnimation));
        }
        else if (state == AbilityState.Active) // Can only transition to Active from Preview
        {
            XRGrabInteractable previewGrab = Preview.GetComponent<XRGrabInteractable>();

            // Stop listening to the preview's grab/release events
            previewGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
            previewGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);

            // If the active item is grabbable
            if (EntityView.TryGetComponent(out XRGrabInteractable activeGrab))
            {
                // Force it into the player's hand, and listen for its grab/release events
                activeGrab.interactionManager.SelectEnter(previewGrab.GetNewestInteractorSelecting(), activeGrab);
                activeGrab.firstSelectEntered.AddListener(ForwardItemGrabbedEvent);
                activeGrab.lastSelectExited.AddListener(ForwardItemReleasedEvent);
            }

            await Preview.PlayHideAnimation(RefreshAnimationToken(ref _previewAnimation));
            Preview.Hide();

            EntityView.Show();
            await EntityView.PlayShowAnimation(RefreshAnimationToken(ref _entityViewAnimation));
        }
        else if (state == AbilityState.Consumed) // Can only transition to Consumed from Active
        {
            Destroy(Card);
            Destroy(Preview);

            // If the active item is grabbable
            if (EntityView.TryGetComponent(out XRGrabInteractable activeGrab))
            {
                // Stop listening to its grab/release events
                activeGrab.firstSelectEntered.RemoveListener(ForwardItemGrabbedEvent);
                activeGrab.lastSelectExited.RemoveListener(ForwardItemReleasedEvent);
            }

            await EntityView.PlayHideAnimation(RefreshAnimationToken(ref _entityViewAnimation));
            Destroy(EntityView);
        }

    }

    /// <summary>
    /// Stops an existing animation task if it is running, then creates and returns a new animation token
    /// </summary>
    /// <param name="animationTokenSource"></param>
    /// <returns></returns>
    private CancellationToken RefreshAnimationToken(ref CancellationTokenSource animationTokenSource)
    {
        if (animationTokenSource != null)
        {
            animationTokenSource.Cancel();
            animationTokenSource.Dispose();
        }

        animationTokenSource = new CancellationTokenSource();
        return animationTokenSource.Token;
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