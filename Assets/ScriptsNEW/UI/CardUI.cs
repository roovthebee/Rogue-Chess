using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler
{
    // Serialized Fields

    [SerializeField] private Image artworkImage;

    [SerializeField] private TMP_Text nameText;

    [SerializeField] private Button button;

    [SerializeField] private GameObject selectionHighlight;

    // Private Fields

    private Card card;

    private CardManager cardManager;

    private CardInfoPanelUI infoPanel;

    // Public Properties

    public Card Card => card;

    // Unity Messages

    private void Awake()
    {
        button.onClick.AddListener(OnClicked);

        selectionHighlight.SetActive(false);
    }

    // Public Methods

    public void Initialize(Card card, CardManager cardManager, CardInfoPanelUI infoPanel)
    {
        this.card = card;
        this.cardManager = cardManager;
        this.infoPanel = infoPanel;

        CardData data = card.CardData;

        artworkImage.sprite = data.Artwork;
        nameText.text = data.CardName;
    }

    public void SetSelected(bool selected)
    {
        selectionHighlight.SetActive(selected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.Show(card);
    }

    // Private Event Handlers

    private void OnClicked()
    {
        cardManager.SelectCard(card);
    }
}