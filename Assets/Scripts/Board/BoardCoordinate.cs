using UnityEngine;

namespace RogueChess.Board
{
    [System.Serializable]
    public struct BoardCoordinate
    {
        public int X;
        public int Y;

        public BoardCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}