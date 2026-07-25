using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private RectTransform presentation;
    [SerializeField] private TMP_Text cardName;

    [Header("Animation")]
    [SerializeField] private float hoverHeight = 25f;
    [SerializeField] private float selectedHeight = 45f;
    [SerializeField] private float hoverScale = 1f;
    [SerializeField] private float selectedScale = 1.05f;
    [SerializeField] private float smoothTime = 0.08f;

    private Card card;
    
    private bool hovered;
    private bool selected;

    private float positionVelocity;
    private Vector3 scaleVelocity;

    public Card Card => card;

    public void Initialize(Card card)
    {
        this.card = card;

        cardName.text = card.CardData.CardName;
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;

        if (selected)
            hovered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selected)
            return;

        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selected)
            return;

        hovered = false;
    }

    private void Update()
    {
        float targetY = 0f;

        if (selected)
            targetY = selectedHeight;
        else if (hovered)
            targetY = hoverHeight;

        Vector2 anchoredPosition = presentation.anchoredPosition;

        anchoredPosition.y = Mathf.SmoothDamp(anchoredPosition.y, targetY, ref positionVelocity, smoothTime);
        
        presentation.anchoredPosition = anchoredPosition;

        Vector3 targetScale = selected ? Vector3.one * selectedScale : Vector3.one * hoverScale;

        presentation.localScale = Vector3.SmoothDamp(presentation.localScale, targetScale, ref scaleVelocity, smoothTime);
    }
}