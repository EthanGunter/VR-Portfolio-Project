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

        // TODO wait for game start animation to finish
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

        IEnumerable<AbilityData> addedData = DrawPile.Draw(num);
        foreach (AbilityData data in addedData)
        {
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
        if (ability.State == AbilityState.Active)
        {
            // Discard the ability card
            Discard(ability);
        }
    }

    #endregion
}