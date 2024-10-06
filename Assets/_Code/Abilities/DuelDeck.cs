using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class DuelDeck : MonoBehaviour
{
    #region Variables

    [SerializeField] AbilitySet starterDeck;

    [SerializeField] int handSize;

    public readonly Deck<AbilityData> DrawPile = new Deck<AbilityData>();
    public readonly Deck<AbilityData> Hand = new Deck<AbilityData>();
    public readonly Deck<AbilityData> DiscardPile = new Deck<AbilityData>();

    #endregion


    #region Unity Messages

    private void Start()
    {
        DrawPile.Add(starterDeck.CreateRuntimeCopy());

        // TODO wait for game start animation (doesn't exist yet) to finish
        Draw(handSize);
    }

    #endregion


    public void Draw(int num, bool skipAnimation = false)
    {
        if (DrawPile.Count < num)
        {
            DrawPile.Add(DiscardPile.Draw(-1));
            DrawPile.Shuffle();
        }

        //Draws a new hand, even if the ability is still active...
        IEnumerable<AbilityData> addedData = DrawPile.Draw(num);
        foreach (AbilityData data in addedData)
        {
            data.ChangeState(AbilityState.Card);
            data.OnStateChange += AbilityStateChanged;
        }
        Hand.Add(addedData);
    }


    private void Discard(AbilityData ability)
    {
        ability.OnStateChange -= AbilityStateChanged;

        Hand.Remove(ability);
        DiscardPile.Add(ability);

        if (Hand.Count == 0)
        {
            Draw(handSize);
        }
    }


    #region Event Handlers

    private void AbilityStateChanged(AbilityData ability)
    {
        HandleStateChangeV2(ability);
    }

    /// <summary>
    /// Discards cards as soon as their ability has been activated.
    /// <para>WARNING: As of 9/18/24, this function causes state issues because of how/when <see cref="AbilityData.Initialize"/> is called...</para>
    /// </summary>
    private void HandleStateChangeV1(AbilityData ability)
    {
        if (ability.State == AbilityState.Active)
        {
            ability.OnStateChange -= AbilityStateChanged;

            Hand.Remove(ability);
            DiscardPile.Add(ability);

            if (Hand.Count == 0)
            {
                Draw(handSize);
            }
        }
    }

    /// <summary>
    /// Removes cards from the hand when activated, but doesn't discard until the ability has been consumed
    /// </summary>
    private void HandleStateChangeV2(AbilityData ability)
    {
        if (ability.State == AbilityState.Active)
        {
            Hand.Remove(ability);

            if (Hand.Count == 0)
            {
                Draw(handSize);
            }
        }
        else if (ability.State == AbilityState.Consumed)
        {
            ability.OnStateChange -= AbilityStateChanged;

            DiscardPile.Add(ability);

            if (Hand.Count == 0)
            {
                Draw(handSize);
            }
        }
    }

    #endregion
}