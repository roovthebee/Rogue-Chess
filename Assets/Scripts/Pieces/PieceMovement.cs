using System.Collections.Generic;
using RogueChess.Board;
using RogueChess.Core.Rules;
using UnityEngine;

namespace RogueChess.Pieces
{
    public static class PieceMovement
    {
        public static List<MoveData> GetMoves(Piece piece, BoardManager boardManager)
        {
            List<MoveData> moves = GetPseudoLegalMoves(piece, boardManager);

            if (piece.PieceData.PieceType == PieceType.King)
            {
                AddCastleMoves(piece, boardManager, moves);
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

        private static void ProcessSpecialRule(Piece piece, BoardManager boardManager, MovementRule rule, List<MoveData> moves)
        {
            switch (piece.PieceData.PieceType)
            {
                case PieceType.Pawn:
                    GeneratePawnMoves(piece, boardManager, moves);
                    break;
            }
        }

        private static void GeneratePawnMoves(Piece piece, BoardManager boardManager, List<MoveData> moves)
        {
            int direction = piece.Team == Team.White ? 1 : -1;
            BoardCoordinate forwardSquare = new BoardCoordinate(piece.Coordinate.X, piece.Coordinate.Y + direction);

            if (boardManager.IsWithinBounds(forwardSquare) && !boardManager.IsTileOccupied(forwardSquare))
            {
                moves.Add(new MoveData(forwardSquare, false));
            }

            if (!piece.HasMoved && !boardManager.IsTileOccupied(forwardSquare))
            {
                BoardCoordinate doubleForwardSquare = new BoardCoordinate(piece.Coordinate.X, piece.Coordinate.Y + direction * 2);

                if (boardManager.IsWithinBounds(doubleForwardSquare) && !boardManager.IsTileOccupied(doubleForwardSquare))
                {
                    moves.Add(new MoveData(doubleForwardSquare, false));
                }
            }

            BoardCoordinate leftCapture = new BoardCoordinate(piece.Coordinate.X - 1, piece.Coordinate.Y + direction);
            BoardCoordinate rightCapture = new BoardCoordinate(piece.Coordinate.X + 1, piece.Coordinate.Y + direction);

            TryAddPawnCapture(piece, boardManager, leftCapture, moves);
            TryAddPawnCapture(piece, boardManager, rightCapture, moves);
        }

        private static void TryAddPawnCapture(Piece piece, BoardManager boardManager, BoardCoordinate coordinate, List<MoveData> moves)
        {
            if (!boardManager.IsWithinBounds(coordinate))
            {
                return;
            }

            Piece targetPiece = boardManager.GetPieceAtCoordinate(coordinate);

            if (targetPiece == null)
            {
                return;
            }

            if (targetPiece.Team != piece.Team)
            {
                moves.Add(new MoveData(coordinate, true));
            }
        }

        public static List<BoardCoordinate> GetThreatenedSquares(Piece piece, BoardManager boardManager)
        {
            if (piece.PieceData.PieceType == PieceType.Pawn)
            {
                return GetPawnThreatenedSquares(piece);
            }

            List<BoardCoordinate> threatenedSquare = new List<BoardCoordinate>();

            foreach (MoveData move in GetPseudoLegalMoves(piece, boardManager))
            {
                threatenedSquare.Add(move.TargetCoordinate);
            }

            return threatenedSquare;
        }

        private static List<MoveData> GetPseudoLegalMoves(Piece piece, BoardManager boardManager)
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
                    
                    case MovementType.Special:
                        ProcessSpecialRule(piece, boardManager, rule, moves);
                        break;
                }
            }

            return moves;
        }

        private static List<BoardCoordinate> GetPawnThreatenedSquares(Piece piece)
        {
            List<BoardCoordinate> threatenedSquares = new List<BoardCoordinate>();
            int direction = piece.Team == Team.White ? 1 : -1;

            threatenedSquares.Add(new BoardCoordinate(piece.Coordinate.X - 1, piece.Coordinate.Y + direction));
            threatenedSquares.Add(new BoardCoordinate(piece.Coordinate.X + 1, piece.Coordinate.Y + direction));

            return threatenedSquares;
        }

        private static void AddCastleMoves(Piece king, BoardManager boardManager, List<MoveData> moves)
        {
            if (king.HasMoved)
            {
                return;
            }

            if (ChessRules.IsKingInCheck(king.Team, boardManager))
            {
                return;
            }

            TryAddKingSideCastle(king, boardManager, moves);
            TryAddQueenSideCastle(king, boardManager, moves);
        }

        private static void TryAddKingSideCastle(Piece king, BoardManager boardManager, List<MoveData> moves)
        {
            BoardCoordinate rookCoordinate = new BoardCoordinate(boardManager.BoardWidth - 1, king.Coordinate.Y);
            Piece rook = boardManager.GetPieceAtCoordinate(rookCoordinate);

            if (rook == null)
            {
                return;
            }

            if (rook.PieceData.PieceType != PieceType.Rook)
            {
                return;
            }

            if (rook.Team != king.Team)
            {
                return;
            }

            if (rook.HasMoved)
            {
                return;
            }

            BoardCoordinate firstSquare = new BoardCoordinate(king.Coordinate.X + 1, king.Coordinate.Y);
            BoardCoordinate secondSquare = new BoardCoordinate(king.Coordinate.X + 2, king.Coordinate.Y);

            if (boardManager.IsTileOccupied(firstSquare))
            {
                return;
            }

            if (boardManager.IsTileOccupied(secondSquare))
            {
                return;
            }

            if (ChessRules.IsSquareThreatened(firstSquare, king.Team, boardManager))
            {
                return;
            }

            if (ChessRules.IsSquareThreatened(secondSquare, king.Team, boardManager))
            {
                return;
            }

            moves.Add(new MoveData(secondSquare, false, SpecialMoveType.CastleKingSide));
        }

        private static void TryAddQueenSideCastle(Piece king, BoardManager boardManager, List<MoveData> moves)
        {
            BoardCoordinate rookCoordinate = new BoardCoordinate(0, king.Coordinate.Y);
            Piece rook = boardManager.GetPieceAtCoordinate(rookCoordinate);

            if (rook == null)
            {
                return;
            }

            if (rook.PieceData.PieceType != PieceType.Rook)
            {
                return;
            }

            if (rook.Team != king.Team)
            {
                return;
            }

            if (rook.HasMoved)
            {
                return;
            }

            BoardCoordinate firstSquare = new BoardCoordinate(king.Coordinate.X - 1, king.Coordinate.Y);
            BoardCoordinate secondSquare = new BoardCoordinate(king.Coordinate.X - 2, king.Coordinate.Y);
            BoardCoordinate thirdSquare = new BoardCoordinate(king.Coordinate.X - 3, king.Coordinate.Y);

            if (boardManager.IsTileOccupied(firstSquare))
            {
                return;
            }

            if (boardManager.IsTileOccupied(secondSquare))
            {
                return;
            }

            if (boardManager.IsTileOccupied(thirdSquare))
            {
                return;
            }

            if (ChessRules.IsSquareThreatened(firstSquare, king.Team, boardManager))
            {
                return;
            }

            if (ChessRules.IsSquareThreatened(secondSquare, king.Team, boardManager))
            {
                return;
            }

            moves.Add(new MoveData(secondSquare, false, SpecialMoveType.CastleQueenSide));
        }
    }
}