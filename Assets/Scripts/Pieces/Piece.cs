using RogueChess.Board;
using UnityEngine;

namespace RogueChess.Pieces
{
    public class Piece : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        public PieceData PieceData { get; private set; }
        
        public Team Team { get; private set; }
        
        public BoardCoordinate Coordinate { get; private set; }

        public void Initialize(PieceData pieceData, Team team, BoardCoordinate coordinate)
        {
            PieceData = pieceData;
            Team = team;
            Coordinate = coordinate;

            spriteRenderer.sprite = pieceData.PieceSprite;
        }

        public void SetCoordinate(BoardCoordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}