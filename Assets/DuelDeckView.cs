using System;
using TMPro;
using UnityEngine;

public class DuelDeckView : MonoBehaviour, IView<DuelDeck>
{
    [SerializeField] private DuelDeck deck;
    [SerializeField] private TextMeshProUGUI handText;
    [SerializeField] private TextMeshProUGUI drawPileText;
    [SerializeField] private TextMeshProUGUI discardPileText;
    public DuelDeck Model => deck;

    private void OnEnable()
    {
        deck.handChanged.AddListener(handChanged);
        deck.drawPileChanged.AddListener(drawPileChanged);
        deck.discardPileChanged.AddListener(discardPileChanged);
    }

    private void OnDisable()
    {
        deck.handChanged.RemoveListener(handChanged);
        deck.drawPileChanged.RemoveListener(drawPileChanged);
        deck.discardPileChanged.RemoveListener(discardPileChanged);
    }

    private void handChanged(Deck drawPile)
    {
        handText.text = $"Hand: {drawPile.Count}";
    }
    private void drawPileChanged(Deck drawPile)
    {
        drawPileText.text = $"Deck: {drawPile.Count}";
    }

    private void discardPileChanged(Deck discardPile)
    {
        discardPileText.text = $"Discard: {discardPile.Count}";
    }

    public void Hide() { }
    public void Show() { }
}
