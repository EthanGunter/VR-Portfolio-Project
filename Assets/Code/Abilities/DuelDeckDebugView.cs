using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DuelDeckDebugView : MonoBehaviour
{
    [SerializeField] DuelDeck deck;
    [SerializeField] private TextMeshProUGUI handText;
    [SerializeField] private TextMeshProUGUI drawPileText;
    [SerializeField] private TextMeshProUGUI discardPileText;

    private void OnEnable()
    {
        deck.Hand.OnCardsAdded += handChanged;
        deck.Hand.OnCardsRemoved += handChanged;
        deck.DrawPile.OnCardsAdded += drawPileChanged;
        deck.DrawPile.OnCardsRemoved += drawPileChanged;
        deck.DiscardPile.OnCardsAdded += discardPileChanged;
        deck.DiscardPile.OnCardsRemoved += discardPileChanged;
    }

    private void OnDisable()
    {
        deck.Hand.OnCardsAdded -= handChanged;
        deck.Hand.OnCardsRemoved -= handChanged;
        deck.DrawPile.OnCardsAdded -= drawPileChanged;
        deck.DrawPile.OnCardsRemoved -= drawPileChanged;
        deck.DiscardPile.OnCardsAdded -= discardPileChanged;
        deck.DiscardPile.OnCardsRemoved -= discardPileChanged;
    }

    private void handChanged(IEnumerable<AbilityData> ad)
    {
        handText.text = $"Hand: {deck.Hand.Count}";
    }
    private void drawPileChanged(IEnumerable<AbilityData> ad)
    {
        drawPileText.text = $"Deck: {deck.DrawPile.Count}";
    }

    private void discardPileChanged(IEnumerable<AbilityData> ad)
    {
        discardPileText.text = $"Discard: {deck.DiscardPile.Count}";
    }
}