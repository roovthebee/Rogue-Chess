using RogueChess.Board;

namespace RogueChess.Pieces
{
    public struct MoveData
    {
        public BoardCoordinate TargetCoordinate;

        public bool IsOccupied;

        public MoveData(BoardCoordinate targetCoordinate, bool isOccupied)
        {
            TargetCoordinate = targetCoordinate;
            IsOccupied = isOccupied;
        }
    }
}