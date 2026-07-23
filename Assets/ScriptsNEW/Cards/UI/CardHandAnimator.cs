using UnityEngine;
using UnityEngine.EventSystems;

namespace RogueChess.UI
{
    public class CardHandAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Animation")]
        [SerializeField] private RectTransform cardContainer;

        [SerializeField] private float hiddenY = -170f;
        [SerializeField] private float shownY = -30f;
        [SerializeField] private float smoothTime = 0.15f;

        private bool expanded;
        private float velocity;

        private void Update()
        {
            Vector2 position = cardContainer.anchoredPosition;

            float targetY = expanded ? shownY : hiddenY;

            position.y = Mathf.SmoothDamp(position.y, targetY, ref velocity, smoothTime);

            cardContainer.anchoredPosition = position;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            expanded = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            expanded = false;
        }
    }
}