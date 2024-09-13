using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public delegate void AbilitySelectEvent(AbilityData ability, IXRSelectInteractor interactor);

[CreateAssetMenu(menuName = "Abilities/Ability")]
public class AbilityData : ScriptableObject
{
    #region Compile-time Inspector fields

    [SerializeField] private Card _cardPrefab;
    [SerializeField] private Preview _previewPrefab;
    [SerializeField] private Ability _abilityPrefab;

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
                _card.Initialize(this);
            }
            return _card;
        }
        private set { _card = value; }
    }
    private Card _card;
    public Preview Preview
    {
        get
        {
            if (_preview == null)
            {
                _preview = Instantiate(_previewPrefab);
                _preview.Initialize(this);
            }
            return _preview;
        }
        private set { _preview = value; }
    }
    private Preview _preview;
    public Ability Ability
    {
        get
        {
            if (_ability == null)
            {
                _ability = Instantiate(_abilityPrefab);
                _ability.Initialize(this);
            }
            return _ability;
        }
        private set { _ability = value; }
    }
    private Ability _ability;


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
    // Card <-> Preview -> Active
    public async Awaitable ChangeState(AbilityState state)
    {
        if (State == state)
        {
            return;
        }

        State = state;

        if (state == AbilityState.Card)
        {
            Preview.Grabbable.firstSelectEntered.RemoveListener(ForwardItemGrabbed);
            Preview.Grabbable.lastSelectExited.RemoveListener(ForwardItemReleased);

            Card.Show();
            Card.AttachToHand(Preview.HeldBy);
            Card.Grabbable.firstSelectEntered.AddListener(ForwardItemGrabbed);
            Card.Grabbable.lastSelectExited.AddListener(ForwardItemReleased);

            Preview.Hide();
        }
        else if (state == AbilityState.Preview)
        {
            Card.Grabbable.firstSelectEntered.RemoveListener(ForwardItemGrabbed);
            Card.Grabbable.lastSelectExited.RemoveListener(ForwardItemReleased);

            Preview.Show();
            Preview.AttachToHand(Card.HeldBy);
            Preview.Grabbable.firstSelectEntered.AddListener(ForwardItemGrabbed);
            Preview.Grabbable.lastSelectExited.AddListener(ForwardItemReleased);

            Card.Hide();
        }
        else if (state == AbilityState.Active)
        {
            Preview.Grabbable.firstSelectEntered.RemoveListener(ForwardItemGrabbed);
            Preview.Grabbable.lastSelectExited.RemoveListener(ForwardItemReleased);
            Ability.Show();
            Ability.AttachToHand(Preview.HeldBy);

            Preview.Hide();
            //Task[] awaitables = new Task[2];
            //awaitables[0] = Card.Hide();
            //awaitables[1] = Preview.Hide();
            //await Task.WaitAll(awaitables);
        }
        else if (state == AbilityState.Consumed)
        {
            Destroy(Card);
            Destroy(Preview);

            await Ability.Hide();
            Destroy(Ability);
        }

    }

    private void ForwardItemGrabbed(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out AbilityComponent ac))
        {
            OnActiveItemGrabbed?.Invoke(ac.Ability, args.interactorObject);
        }
        else
        {
            throw new InvalidOperationException($"{args.interactableObject.transform.name} has no AbilityComponent on it. Cannot forward Grabbed event");
        }
    }
    private void ForwardItemReleased(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out AbilityComponent ac))
        {
            OnActiveItemReleased?.Invoke(ac.Ability, args.interactorObject);
        }
        else
        {
            throw new InvalidOperationException($"{args.interactableObject.transform.name} has no AbilityComponent on it. Cannot forward Released event");
        }
    }
}