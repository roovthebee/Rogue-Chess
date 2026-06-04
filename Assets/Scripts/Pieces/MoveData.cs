using RogueChess.Board;

namespace RogueChess.Pieces
{
    public struct MoveData
    {
        public BoardCoordinate TargetCoordinate;

        public bool IsCapture;

        public MoveData(BoardCoordinate targetCoordinate, bool isCapture)
        {
            TargetCoordinate = targetCoordinate;
            IsCapture = isCapture;
        }
    }
}