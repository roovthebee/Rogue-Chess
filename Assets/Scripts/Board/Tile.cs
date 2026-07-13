using System;
using UnityEngine;

namespace RogueChess.Board
{
    public class Tile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject moveIndicator;
        [SerializeField] private GameObject captureIndicator;

        private Color defaultColor;

        public BoardCoordinate Coordinate { get; private set; }

        public static event Action<Tile> OnTileClicked;

        public void Initialize(BoardCoordinate coordinate)
        {
            Coordinate = coordinate;
        }

        public void SetColor(Color color)
        {
            defaultColor = color;
            spriteRenderer.color = color;
        }

        public void ResetColor()
        {
            spriteRenderer.color = defaultColor;
        }

        public void SetTemporaryColor(Color color)
        {
            spriteRenderer.color = color;
        }

        private void OnMouseDown()
        {
            OnTileClicked?.Invoke(this);
        }

        public void ShowMoveIndicator()
        {
            moveIndicator.SetActive(true);
        }

        public void HideMoveIndicator()
        {
            moveIndicator.SetActive(false);
        }

        public void ShowCaptureIndicator()
        {
            captureIndicator.SetActive(true);
        }

        public void HideCaptureIndicator()
        {
            captureIndicator.SetActive(false);
        }
    }
}
