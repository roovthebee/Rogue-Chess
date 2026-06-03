using System;
using UnityEngine;

namespace RogueChess.Board
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject moveIndicator;

        public BoardCoordinate Coordinate { get; private set; }

        public static event Action<Tile> OnTileClicked;

        public void Initialize(BoardCoordinate coordinate)
        {
            Coordinate = coordinate;
        }

        public void SetColor(Color color)
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
    }
}
