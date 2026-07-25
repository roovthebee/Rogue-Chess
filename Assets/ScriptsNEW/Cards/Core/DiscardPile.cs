using System.Collections.Generic;

public class DiscardPile
{
    // Private Fields

    private readonly List<Card> cards = new();

    // Public Properties

    public IReadOnlyList<Card> Cards => cards;

    public int Count => cards.Count;

    // Public Methods

    public void Add(Card card)
    {
        if (card == null)
        {
            return;
        }

        cards.Add(card);
    }

    public Card DrawTop()
    {
        if (cards.Count == 0)
        {
            return null;
        }

        Card card = cards[^1];

        cards.RemoveAt(cards.Count - 1);

        return card;
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