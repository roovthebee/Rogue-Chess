using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Serialized Fields

    [SerializeField] private Image artworkImage;

    [SerializeField] private TMP_Text nameText;

    [SerializeField] private Button button;

    [SerializeField] private GameObject selectionHighlight;

    // Private Fields

    private Card card;

    private CardManager cardManager;

    // Public Properties

    public Card Card => card;

    // Events

    public event Action<Card> PointerEntered;

    public event Action<Card> PointerExited;

    // Public Methods

    public void Initialize(Card card, CardManager cardManager)
    {
        this.card = card;
        this.cardManager = cardManager;

        CardData data = card.CardData;

        artworkImage.sprite = data.Artwork;
        nameText.text = data.CardName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    public void SetSelected(bool selected)
    {
        selectionHighlight.SetActive(selected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEntered?.Invoke(card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExited?.Invoke(card);
    }

    // Private Event Handlers

    private void OnClicked()
    {
        cardManager.SelectCard(card);
    }
}