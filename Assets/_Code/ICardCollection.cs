
using System.Collections.Generic;

public interface ICardCollection<T> : ICollection<T>
{
    /// <summary>
    /// Adds cards to the collection
    /// </summary>
    /// <param name="cards">A collection of cards to add</param>
    /// <param name="top">Place cards on top of the collection, or bottom</param>
    void Add(IEnumerable<T> cards, bool top);
    IEnumerable<T> Draw(int count);
    void Shuffle();
    T this[int index] { get; }
}