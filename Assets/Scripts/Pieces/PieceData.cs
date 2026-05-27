using UnityEngine;

namespace RogueChess.Pieces
{
    [CreateAssetMenu(fileName = "PieceData", menuName = "RogueChess/Piece Data")]
    public class PieceData : ScriptableObject
    {
        public string PieceName;
        public Sprite PieceSprite;
    }
}