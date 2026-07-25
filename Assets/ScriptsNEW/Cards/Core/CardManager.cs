using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private BoardManager boardManager;

    [SerializeField] private List<CardData> startingDeck;

    [SerializeField] private NetworkGameManager networkGameManager;

    [SerializeField] private SelectionManager selectionManager;

    [SerializeField] private ChessRules chessRules;

    // Private Fields

    private readonly Dictionary<int, Card> cardLookup = new();

    private readonly Deck deck = new();
    
    private readonly Hand hand = new();

    private readonly DiscardPile discardPile = new();

    private Card selectedCard;

    private CardPhase currentPhase;

    private readonly CardTargetService targetService = new();

    private bool canPlayCards;

    private bool hasPlayedCardThisTurn;

    // Public Properties

    public Deck Deck => deck;

    public Hand Hand => hand;

    public DiscardPile DiscardPile => discardPile;

    public Card SelectedCard => selectedCard;

    public CardPhase CurrentPhase => currentPhase;

    public bool IsTargeting => currentPhase == CardPhase.Targeting;

    public bool CanPlayCards => canPlayCards;

    // Events

    public event Action<Hand> HandChanged;

    public event Action<Card> CardSelected;

    public event Action<CardPhase> PhaseChanged;

    // Unity Messages

    private void Start()
    {
        foreach (CardData cardData in startingDeck)
        {
            Card card = new(cardData);

            deck.Add(card);
            cardLookup[cardData.CardId] = card;
        }

        deck.Shuffle();

        BeginTurn();
    }

    // Public Methods

    public Card CreateCard(int cardId)
    {
        if (!cardLookup.TryGetValue(cardId, out Card card))
        {
            return null;
        }

        return card;
    }

    public void CancelSelection()
    {
        if (selectedCard == null)
        {
            return;
        }

        ClearSelection();

        EnterPlayPhase();
    }

    public void SelectCard(Card card)
    {
        Team team = PlayerRoleManager.Instance.LocalTeam;

        if (GameManager.Instance.CurrentTurn != team)
        {
            return;
        }

        if (chessRules.IsKingInCheck(team))
        {
            return;
        }

        if (!CanSelectCard(card))
        {
            return;
        }

        if (selectedCard == card)
        {
            CancelSelection();
            return;
        }

        if (selectedCard != null)
        {
            ClearSelection();
        }

        selectionManager.ClearSelection();

        SetSelectedCard(card);

        if (card.CardData.TargetType == TargetType.None)
        {
            ResolveSelectedCard(null);

            return;
        }

        EnterTargetingPhase();
    }

    public void SelectTarget(CardTarget target)
    {
        if (!CanTarget(target))
        {
            return;
        }

        ResolveSelectedCard(target);
    }

    public void BeginTurn()
    {
        canPlayCards = true;

        hasPlayedCardThisTurn = false;

        CancelSelection();

        while (!hand.IsFull)
        {
            Card card = deck.Draw();

            if (card == null)
            {
                break;
            }

            hand.Add(card);
        }

        HandChanged?.Invoke(hand);

        EnterPlayPhase();
    }

    public void EndTurn()
    {
        canPlayCards = false;

        CancelSelection();
    }

    public void EnableCardPlay()
    {
        canPlayCards = true;
    }

    public void DisableCardPlay()
    {
        canPlayCards = false;

        CancelSelection();
    }

    public void SetHand(IEnumerable<Card> cards)
    {
        hand.SetCards(cards);

        HandChanged?.Invoke(hand);
    }

    public bool IsHost()
    {
        return networkGameManager.IsHost;
    }

    public void CompleteCardPlay()
    {
        hand.Remove(selectedCard);

        discardPile.Add(selectedCard);

        HandChanged?.Invoke(hand);

        ClearSelection();

        EnterPlayPhase();
    }

    // Private Workflow

    private void EnterPlayPhase()
    {
        SetPhase(CardPhase.Play);
    }

    private void EnterTargetingPhase()
    {
        SetPhase(CardPhase.Targeting);
    }

    private void ResolveSelectedCard(CardTarget target)
    {
        networkGameManager.ExecuteCard(selectedCard, target);
    }

    // Private State

    private void SetPhase(CardPhase phase)
    {
        if (currentPhase == phase)
        {
            return;
        }

        currentPhase = phase;

        PhaseChanged?.Invoke(currentPhase);
    }

    private void SetSelectedCard(Card card)
    {
        if (selectedCard == card)
        {
            return;
        }

        selectedCard = card;

        CardSelected?.Invoke(selectedCard);
    }

    private void ClearSelection()
    {
        if (selectedCard == null)
        {
            return;
        }

        selectedCard = null;

        CardSelected?.Invoke(selectedCard);
    }

    // Private Validation

    private bool CanSelectCard(Card card)
    {
        if (card == null)
        {
            return false;
        }

        if (!canPlayCards)
        {
            return false;
        }

        if (hasPlayedCardThisTurn)
        {
            return false;
        }

        if (CurrentPhase != CardPhase.Play && CurrentPhase != CardPhase.Targeting)
        {
            return false;
        }

        return hand.Contains(card);
    }

    private bool CanTarget(CardTarget target)
    {
        if (selectedCard == null)
        {
            return false;
        }

        return targetService.IsValidTarget(selectedCard, target);
    }

    // Private Helpers

}