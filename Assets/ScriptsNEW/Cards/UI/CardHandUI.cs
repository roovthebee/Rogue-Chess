using System.Collections.Generic;
using UnityEngine;

public class CardHandUI : MonoBehaviour
{
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform cardContainer;

    private readonly List<CardView> cardViews = new();

    public void RebuildHand(IReadOnlyList<Card> hand)
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        cardViews.Clear();

        foreach (Card card in hand)
        {
            CardView view = Instantiate(cardPrefab, cardContainer);

            view.Initialize(card);

            cardViews.Add(view);
        }
    }

    public void SelectCard(Card selectedCard)
    {
        foreach (CardView view in cardViews)
        {
            view.SetSelected(view.Card == selectedCard);
        }
    }

    public void ClearSelection()
    {
        foreach (CardView view in cardViews)
        {
            view.SetSelected(false);
        }
    }
}