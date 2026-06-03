using System.Collections.Generic;
using RogueChess.Board;

namespace RogueChess.Pieces
{
    public static class PieceMovement
    {
        public static List<MoveData> GetMoves(Piece piece, BoardManager boardManager)
        {
            List<MoveData> moves = new List<MoveData>();

            foreach (MovementRule rule in piece.PieceData.MovementRules)
            {
                BoardCoordinate coordinate = new BoardCoordinate(piece.Coordinate.X + rule.Direction.x, piece.Coordinate.Y + rule.Direction.y);

                if (boardManager.IsWithinBounds(coordinate))
                {
                    moves.Add(new MoveData(coordinate));
                }
            }

            return moves;
        }
    }
}