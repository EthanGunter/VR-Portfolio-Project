using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


public class Deck<T> : ICardCollection<T>
{
    #region Variables

    //public event Action OnOrderChange;
    public event Action<IEnumerable<T>> OnCardsAdded;
    public event Action<IEnumerable<T>> OnCardsRemoved;
    public int Count => _cards.Count;
    public bool IsReadOnly => ((ICollection<T>)_cards).IsReadOnly;

    List<T> _cards = new List<T>();
    System.Random _rand = new System.Random();

    #endregion

    public Deck(List<T> cards = null)
    {
        if (cards == null)
        {
            _cards = new List<T>();
        }
        else
        {
            _cards = cards;
        }
    }

    // TODO Test...
    public void Add(IEnumerable<T> cards, bool top)
    {
        // index 0 is considered the top of the collection
        if (top)
        {
            List<T> newCards = new List<T>();
            newCards.AddRange(cards);
            newCards.AddRange(_cards);
            _cards = newCards;
        }
        else
        {
            _cards.AddRange(cards);
        }
    }

    /// <summary>
    /// Draw cards from the deck
    /// </summary>
    /// <param name="count">Values less than 0 take all</param>
    /// <returns></returns>
    public IEnumerable<T> Draw(int count = 1)
    {
        if (count < 0)
        {
            // Draw all
            IEnumerable<T> drawnCards = _cards.ToList();
            if (drawnCards.Count() > 0)
            {
                _cards.Clear();
                OnCardsRemoved?.Invoke(drawnCards);
            }
            return drawnCards;
        }
        else
        {
            IEnumerable<T> drawnCards = _cards.Take(count);
            if (drawnCards.Count() > 0)
            {
                _cards = _cards.Skip(count).ToList();
                OnCardsRemoved?.Invoke(drawnCards);
            }
            return drawnCards;
        }
    }

    /// <summary>
    /// Draw cards from the deck with a condition
    /// </summary>
    /// <param name="predicate">The condition that filters what cards get returned</param>
    /// <param name="count">The max number of cards to collect</param>
    /// <returns>As many cards pass the condition, up to <c>count</c></returns>
    public IEnumerable<T> DrawWhere(Func<T, bool> predicate, int count = 1)
    {
        List<T> foundCards = new();
        for (int i = 0; i < _cards.Count; i++)
        {
            if (predicate(_cards[i]))
            {
                foundCards.Add(_cards[i]);
            }
            if (foundCards.Count == count)
                break;
        }

        foreach (var card in foundCards)
        {
            _cards.Remove(card);
        }

        OnCardsRemoved?.Invoke(foundCards);
        return foundCards;
    }

    public void Shuffle()
    {
        int n = _cards.Count;

        while (n > 1)
        {
            int k = _rand.Next(n--);
            T temp = _cards[n];
            _cards[n] = _cards[k];
            _cards[k] = temp;
        }

        //if (n > 0) OnOrderChange?.Invoke();
    }

    public T this[int index]
    {
        get => _cards[index]; set
        {
            if (value == null)
            {
                OnCardsRemoved?.Invoke(new T[] { _cards[index] });
            }
            else
            {
                OnCardsAdded?.Invoke(new T[] { value });
                OnCardsRemoved?.Invoke(new T[] { _cards[index] });
            }

            _cards[index] = value;
        }
    }


    #region ICollection Implementation

    public void Add(T card)
    {
        if (card == null) return;

        ((ICollection<T>)_cards).Add(card);

        OnCardsAdded?.Invoke(new T[] { card });
    }
    public void Add(IEnumerable<T> cards)
    {
        if (cards == null || cards.Count() == 0) return;

        foreach (var card in cards)
        {
            ((ICollection<T>)_cards).Add(card);
        }

        OnCardsAdded?.Invoke(cards);
    }

    public void Clear()
    {
        if (_cards.Count != 0) OnCardsRemoved?.Invoke(_cards);
        ((ICollection<T>)_cards).Clear();
    }

    public bool Contains(T card)
    {
        return ((ICollection<T>)_cards).Contains(card);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)_cards).CopyTo(array, arrayIndex);
    }

    public bool Remove(T card)
    {
        if (((ICollection<T>)_cards).Remove(card))
        {
            OnCardsRemoved?.Invoke(new T[] { card });
            return true;
        }
        else return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_cards).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_cards).GetEnumerator();
    }

    #endregion
}