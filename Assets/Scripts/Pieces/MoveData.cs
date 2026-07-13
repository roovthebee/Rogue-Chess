using RogueChess.Board;

namespace RogueChess.Pieces
{
    public struct MoveData
    {
        public BoardCoordinate TargetCoordinate;

        public bool IsCapture;

        public SpecialMoveType SpecialMoveType;

        public MoveData(BoardCoordinate targetCoordinate, bool isCapture, SpecialMoveType specialMoveType = SpecialMoveType.None)
        {
            TargetCoordinate = targetCoordinate;
            IsCapture = isCapture;
            SpecialMoveType = specialMoveType;
        }
    }
}