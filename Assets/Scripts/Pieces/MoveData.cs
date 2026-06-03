using RogueChess.Board;

namespace RogueChess.Pieces
{
    public struct MoveData
    {
        public BoardCoordinate TargetCoordinate;

        public MoveData(BoardCoordinate targetCoordinate)
        {
            TargetCoordinate = targetCoordinate;
        }
    }
}