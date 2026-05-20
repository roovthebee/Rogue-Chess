using UnityEngine;

namespace RogueChess.Board
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public BoardCoordinate Coordinate { get; private set; }

        private Color defaultColor;

        public void Initialize(BoardCoordinate coordinate)
        {
            Coordinate = coordinate;
            defaultColor = spriteRenderer.color;
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

        public void Highlight(Color highlightColor)
        {
            spriteRenderer.color = highlightColor;
        }

        public void ResetColor()
        {
            spriteRenderer.color = defaultColor;
        }
    }
}
