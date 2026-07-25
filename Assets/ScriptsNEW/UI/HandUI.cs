using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandUI : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private CardManager cardManager;

    [SerializeField] private CardUI cardPrefab;

    [SerializeField] private CardInfoPanelUI infoPanel;

    [SerializeField] private Transform cardContainer;

    [SerializeField] private RectTransform handTransform;

    [SerializeField] private float hiddenY = -120f;

    [SerializeField] private float visibleY = 20f;

    [SerializeField] private float animationSpeed = 8f;

    [SerializeField] private float revealThreshold = 20f;

    // Private Fields

    private readonly List<CardUI> cardUIs = new();

    private bool shouldReveal;

    private bool forceReveal;

    private Card hoveredCard;

    // Unity Messages

    private void Start()
    {
        Vector2 position = handTransform.anchoredPosition;

        position.y = hiddenY;

        handTransform.anchoredPosition = position;
    }

    private void Update()
    {
        UpdateRevealState();
        AnimateHand();
    }

    private void OnEnable()
    {
        cardManager.HandChanged += RefreshHand;
        cardManager.CardSelected += UpdateSelection;
    }

    private void OnDisable()
    {
        cardManager.HandChanged -= RefreshHand;
        cardManager.CardSelected -= UpdateSelection;
    }

    // Private Workflow

    private void UpdateRevealState()
    {
        if (forceReveal)
        {
            shouldReveal = true;
            return;
        }

        if (Mouse.current == null)
        {
            shouldReveal = false;
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        shouldReveal = forceReveal || mousePosition.y <= revealThreshold;
    }

    private void AnimateHand()
    {
        Vector2 position = handTransform.anchoredPosition;

        float targetY = shouldReveal ? visibleY : hiddenY;

        position.y = Mathf.Lerp(position.y, targetY, Time.deltaTime * animationSpeed);

        handTransform.anchoredPosition = position;
    }

    private IEnumerator RevealRoutine()
    {
        forceReveal = true;

        yield return new WaitForSeconds(1.0f);

        if (cardManager.SelectedCard == null)
        {
            forceReveal = false;
        }
    }

    // Private Event Handlers

    private void RefreshHand(Hand hand)
    {
        ClearHand();

        foreach (Card card in hand.Cards)
        {
            CardUI cardUI = Instantiate(cardPrefab, cardContainer);

            cardUI.Initialize(card, cardManager);

            cardUIs.Add(cardUI);

            cardUI.PointerEntered += OnCardHovered;
            cardUI.PointerExited += OnCardHoverEnded;
        }

        UpdateSelection(cardManager.SelectedCard);

        StartCoroutine(RevealRoutine());
    }

    private void UpdateSelection(Card selectedCard)
    {
        foreach (CardUI cardUI in cardUIs)
        {
            cardUI.SetSelected(cardUI.Card == selectedCard);
        }

        RefreshInfoPanel();
    }

    private void OnCardHovered(Card card)
    {
        hoveredCard = card;
        RefreshInfoPanel();
    }

    private void OnCardHoverEnded(Card card)
    {
        if (hoveredCard == card)
        {
            hoveredCard = null;
        }

        RefreshInfoPanel();
    }

    // Private Helpers

    private void RefreshInfoPanel()
    {
        if (hoveredCard != null)
        {
            infoPanel.Show(hoveredCard);
        }
        else if (cardManager.SelectedCard != null)
        {
            infoPanel.Show(cardManager.SelectedCard);
        }
        else
        {
            infoPanel.Hide();
        }
    }

    private void ClearHand()
    {
        foreach (CardUI cardUI in cardUIs)
        {
            Destroy(cardUI.gameObject);
        }

        cardUIs.Clear();
    }
}