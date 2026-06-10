using RogueChess.Board;
using RogueChess.Pieces;
using System.Collections.Generic;

namespace RogueChess.Core.Rules
{
    public static class ChessRules
    {
        public static bool IsSquareThreatened(BoardCoordinate coordinate, Team threatenedTeam, BoardManager boardManager, Piece ignoredPiece = null)
        {
            foreach (Piece piece in boardManager.ActivePieces)
            {
                if (piece == ignoredPiece)
                {
                    continue;
                }

                if (piece.Team == threatenedTeam)
                {
                    continue;
                }

                foreach (BoardCoordinate threatenedSquare in PieceMovement.GetThreatenedSquares(piece, boardManager))
                {
                    if (threatenedSquare.Equals(coordinate))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsKingInCheck(Team team, BoardManager boardManager, Piece ignoredPiece = null)
        {
            foreach (Piece piece in boardManager.ActivePieces)
            {
                if (piece.Team != team)
                {
                    continue;
                }

                if (piece.PieceData.PieceType != PieceType.King)
                {
                    continue;
                }

                return IsSquareThreatened(piece.Coordinate, team, boardManager, ignoredPiece);
            }

            return false;
        }

        public static List<MoveData> GetLegalMoves(Piece piece, BoardManager boardManager)
        {
            List<MoveData> legalMoves = new List<MoveData>();

            foreach (MoveData move in PieceMovement.GetMoves(piece, boardManager))
            {
                if (IsMoveLegal(piece, move, boardManager))
                {
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }

        private static bool IsMoveLegal(Piece piece, MoveData move, BoardManager boardManager)
        {
            BoardCoordinate originalCoordinate = piece.Coordinate;

            Piece capturedPiece = boardManager.GetPieceAtCoordinate(move.TargetCoordinate);
            boardManager.RemovePieceSilently(originalCoordinate);

            if (capturedPiece != null)
            {
                boardManager.RemovePieceSilently(move.TargetCoordinate);
            }

            boardManager.PlacePieceSilently(piece, move.TargetCoordinate);
            piece.SetCoordinateSilently(move.TargetCoordinate);

            bool kingInCheck = IsKingInCheck(piece.Team, boardManager, capturedPiece);

            boardManager.RemovePieceSilently(move.TargetCoordinate);
            boardManager.PlacePieceSilently(piece, originalCoordinate);
            piece.SetCoordinateSilently(originalCoordinate);

            if (capturedPiece != null)
            {
                boardManager.PlacePieceSilently(capturedPiece, move.TargetCoordinate);
            }

            return !kingInCheck;
        }
    }
}