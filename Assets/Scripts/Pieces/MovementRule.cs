using UnityEngine;

namespace RogueChess.Pieces
{
    [CreateAssetMenu(fileName = "MovementRule", menuName = "RogueChess/Movement Rule")]
    public class MovementRule : ScriptableObject
    {
        public MovementType MovementType;
        public Vector2Int Direction;
        public int MaxDistance = 1;
        public bool UnlimitedRange;
    }
}