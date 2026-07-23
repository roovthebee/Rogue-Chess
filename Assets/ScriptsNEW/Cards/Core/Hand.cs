using System.Collections.Generic;

public class Hand
{
    // Constants

    private const int MaximumSize = 5;

    // Private Fields

    private readonly List<Card> cards = new();

    // Public Properties

    public IReadOnlyList<Card> Cards => cards;

    public int Count => cards.Count;

    public bool IsFull => Count >= MaximumSize;

    public int Capacity => MaximumSize;

    public bool IsEmpty => Count == 0;

    // Public Methods

    public bool Add(Card card)
    {
        if (card == null)
        {
            return false;
        }

        if (IsFull)
        {
            return false;
        }

        cards.Add(card);

        return true;
    }

    public bool Remove(Card card)
    {
        return cards.Remove(card);
    }

    public bool Contains(Card card)
    {
        return cards.Contains(card);
    }

    public void Clear()
    {
        cards.Clear();
    }
}