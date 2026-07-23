using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CardInfoPanelUI : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private Image artworkImage;

    [SerializeField] private Image rarityBorderImage;

    [SerializeField] private TMP_Text nameText;

    [SerializeField] private TMP_Text descriptionText;

    // Unity Messages

    private void Awake()
    {
        Clear();
    }

    // Public Method

    public void Show(Card card)
    {
        if (card == null)
        {
            Clear();
            return;
        }

        CardData data = card.CardData;

        artworkImage.sprite = data.Artwork;

        nameText.text = data.CardName;

        descriptionText.text = data.Description;

        rarityBorderImage.color = GetRarityColor(data.Rarity);
    }

    public void Clear()
    {
        artworkImage.sprite = null;

        nameText.text = "No Card Selected";

        descriptionText.text = "Hover over a card to view its description.";

        rarityBorderImage.color = Color.white;
    }

    // Private Helpers

    private Color GetRarityColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                return Color.gray;
            
            case CardRarity.Rare:
                return Color.blue;

            case CardRarity.Legendary:
                return new Color(1f, 0.84f, 0f);

            default:
                return Color.white;
        }
    }
}