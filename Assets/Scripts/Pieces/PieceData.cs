using System.Collections.Generic;
using UnityEngine;

namespace RogueChess.Pieces
{
    [CreateAssetMenu(fileName = "PieceData", menuName = "RogueChess/Piece Data")]
    public class PieceData : ScriptableObject
    {
        public PieceType PieceType;
        public string PieceName;
        public Sprite PieceSprite;
        public List<MovementRule> MovementRules;
    }
}