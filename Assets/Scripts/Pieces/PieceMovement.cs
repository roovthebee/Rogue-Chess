using System.Collections.Generic;
using RogueChess.Board;
using UnityEngine;

namespace RogueChess.Pieces
{
    public static class PieceMovement
    {
        public static List<MoveData> GetMoves(Piece piece, BoardManager boardManager)
        {
            List<MoveData> moves = new List<MoveData>();

            foreach (MovementRule rule in piece.PieceData.MovementRules)
            {
                switch (rule.MovementType)
                {
                    case MovementType.Sliding:
                        ProcessSlidingRule(piece, boardManager, rule, moves);
                        break;

                    case MovementType.Offset:
                        ProcessOffsetRule(piece, boardManager, rule, moves);
                        break;
                }
            }

            return moves;
        }

        private static void ProcessSlidingRule(Piece piece, BoardManager boardManager, MovementRule rule, List<MoveData> moves)
        {
            int maxDistance = rule.UnlimitedRange ? Mathf.Max(8, 8) : rule.MaxDistance;

            for (int distance = 1; distance <= maxDistance; distance++)
            {
                BoardCoordinate coordinate = new BoardCoordinate(piece.Coordinate.X + rule.Direction.x * distance, piece.Coordinate.Y + rule.Direction.y * distance);

                if (!boardManager.IsWithinBounds(coordinate))
                {
                    break;
                }

                Piece occupyingPiece = boardManager.GetPieceAtCoordinate(coordinate);

                if (occupyingPiece == null)
                {
                    moves.Add(new MoveData(coordinate, false));
                    continue;
                }
                
                if (occupyingPiece.Team != piece.Team)
                {
                    moves.Add(new MoveData(coordinate, true));
                }

                break;
            }
        }

        private static void ProcessOffsetRule(Piece piece, BoardManager boardManager, MovementRule rule, List<MoveData> moves)
        {
            BoardCoordinate coordinate = new BoardCoordinate(piece.Coordinate.X + rule.Direction.x, piece.Coordinate.Y + rule.Direction.y);

            if (!boardManager.IsWithinBounds(coordinate))
            {
                return;
            }

            Piece occupyingPiece = boardManager.GetPieceAtCoordinate(coordinate);

            if (occupyingPiece == null)
            {
                moves.Add(new MoveData(coordinate, false));
                return;
            }

            if (occupyingPiece.Team != piece.Team)
            {
                moves.Add(new MoveData(coordinate, true));
            }
        }
    }
}