using UnityEngine;

public class Card
{
    // Private Fields

    private readonly CardData cardData;

    private int currentCost;

    // Public Properties

    public CardData CardData => cardData;

    public int CurrentCost => currentCost;

    // Constructors

    public Card(CardData cardData)
    {
        this.cardData = cardData;

        currentCost = cardData.Cost;
    }

    public bool Resolve(CardContext context)
    {
        foreach (CardEffect effect in cardData.Effects)
        {
            if (!effect.Resolve(context))
            {
                return false;
            }
        }

        return true;
    }

    public void SetCost(int cost)
    {
        currentCost = Mathf.Max(0, cost);
    }

    public void ResetCost()
    {
        currentCost = cardData.Cost;
    }
}