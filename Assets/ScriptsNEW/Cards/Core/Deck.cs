using System.Collections.Generic;

public class Deck
{
    // Private Fields

    private readonly List<Card> cards = new();

    // Public Properties

    public IReadOnlyList<Card> Cards => cards;

    public int Count => cards.Count;

    public bool IsEmpty => cards.Count == 0;

    // Public Methods

    public void Add(Card card)
    {
        if (card == null)
        {
            return;
        }

        cards.Add(card);
    }

    public Card Draw()
    {
        if (cards.Count == 0)
        {
            return null;
        }

        Card card = cards[0];

        cards.RemoveAt(0);

        return card;
    }

    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
        }
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