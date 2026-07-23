using System;
using System.Collections.Generic;
using UnityEngine;

public class ChessRules : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private BoardManager boardManager;

    // Private Fields

    // Public Properties

    // Events

    // Unity Messages

    // Public Methods

    public List<MoveData> GetPseudoLegalMoves(Piece piece)
    {
        return GenerateMoves(piece, MoveGenerationMode.Movement);
    }

    public List<MoveData> GetLegalMoves(Piece piece)
    {
        List<MoveData> pseudoLegalMoves = GetPseudoLegalMoves(piece);

        return FilterLegalMoves(pseudoLegalMoves);
    }

    public bool IsMoveLegal(MoveData move)
    {
        boardManager.ApplyMove(move);

        try
        {
            return !IsKingInCheck(move.Piece.Team);
        }
        finally
        {
            boardManager.RevertMove(move);
        }
    }

    public bool IsKingInCheck(Team team)
    {
        BoardCoordinate kingCoordinate = GetKingCoordinate(team);

        if (kingCoordinate == null)
        {
            return false;
        }

        Team attackingTeam = team == Team.White ? Team.Black : Team.White;

        return IsSquareAttacked(kingCoordinate, attackingTeam);
    }

    public BoardCoordinate GetKingCoordinate(Team team)
    {
        foreach (Piece piece in boardManager.GetActivePieces())
        {
            if (piece.Team == team && piece.PieceData.PieceType == PieceType.King)
            {
                return piece.Coordinate;
            }
        }

        throw new InvalidOperationException("King not found.");
    }

    public bool IsCheckmate(Team team)
    {
        if (!IsKingInCheck(team))
        {
            return false;
        }

        return !TeamHasLegalMove(team);
    }

    public bool IsStalemate(Team team)
    {
        if (IsKingInCheck(team))
        {
            return false;
        }

        return !TeamHasLegalMove(team);
    }

    // Private Workflow

    private List<MoveData> GenerateMoves(Piece piece, MoveGenerationMode mode)
    {
        List<MoveData> moves = new();

        GeneratePieceMoves(piece, moves, mode);

        return moves;
    }

    private void GeneratePieceMoves(Piece piece, List<MoveData> moves, MoveGenerationMode mode)
    {
        switch (piece.PieceData.PieceType)
        {
            case PieceType.Pawn:
                GeneratePawnMoves(piece, moves, mode);
                break;

            case PieceType.Knight:
                GenerateKnightMoves(piece, moves);
                break;

            case PieceType.Bishop:
            case PieceType.Rook:
            case PieceType.Queen:
                GenerateSlidingMoves(piece, moves);
                break;

            case PieceType.King:
                GenerateKingMoves(piece, moves, mode);
                break;
        }
    }

    private List<MoveData> FilterLegalMoves(List<MoveData> pseudoLegalMoves)
    {
        List<MoveData> legalMoves = new();

        foreach (MoveData move in pseudoLegalMoves)
        {
            if (IsMoveLegal(move))
            {
                legalMoves.Add(move);
            }
        }

        return legalMoves;
    }

    private void GenerateSlidingMoves(Piece piece, List<MoveData> moves)
    {
        foreach (MovementRule rule in piece.PieceData.MovementRules)
        {
            GenerateSlidingRule(piece, rule, moves);
        }
    }

    private void GenerateKnightMoves(Piece piece, List<MoveData> moves)
    {
        foreach (MovementRule rule in piece.PieceData.MovementRules)
        {
            BoardCoordinate target = new BoardCoordinate(piece.Coordinate.X + rule.Direction.x, piece.Coordinate.Y + rule.Direction.y);

            EvaluateJumpSquare(piece, target, moves);
        }
    }

    private void GeneratePawnMoves(Piece piece, List<MoveData> moves, MoveGenerationMode mode)
    {
        switch (mode)
        {
            case MoveGenerationMode.Movement:
                GeneratePawnAdvance(piece, moves);
                GeneratePawnDoubleAdvance(piece, moves);
                GeneratePawnCaptures(piece, moves);
                break;
            
            case MoveGenerationMode.Attack:
                GeneratePawnAttackSquares(piece, moves);
                break;
        }
    }

    private void GenerateKingMoves(Piece piece, List<MoveData> moves, MoveGenerationMode mode)
    {
        GenerateNormalKingMoves(piece, moves);

        if (mode == MoveGenerationMode.Movement)
        {
            GenerateCastleMoves(piece, moves);
        }
    }

    private void GenerateNormalKingMoves(Piece piece, List<MoveData> moves)
    {
        foreach (MovementRule rule in piece.PieceData.MovementRules)
        {
            BoardCoordinate target = new BoardCoordinate(piece.Coordinate.X + rule.Direction.x, piece.Coordinate.Y + rule.Direction.y);

            EvaluateJumpSquare(piece, target, moves);
        }
    }

    private void GenerateCastleMoves(Piece king, List<MoveData> moves)
    {
        TryAddKingSideCastle(king, moves);

        TryAddQueenSideCastle(king, moves);
    }

    private void TryAddKingSideCastle(Piece king, List<MoveData> moves)
    {
        int homeRank = king.Team == Team.White ? 0 : 7;

        Piece rook = boardManager.GetPiece(new BoardCoordinate(7, homeRank));

        if (!CanCastle(king, rook))
        {
            return;
        }

        if (!AreCastleSquaresEmpty(king, CastleSide.KingSide))
        {
            return;
        }

        if (!AreCastleSquaresSafe(king, CastleSide.KingSide))
        {
            return;
        }

        moves.Add(new MoveData(king, king.Coordinate, new BoardCoordinate(6, homeRank), null));
    }

    private void TryAddQueenSideCastle(Piece king, List<MoveData> moves)
    {
        int homeRank = king.Team == Team.White ? 0 : 7;

        Piece rook = boardManager.GetPiece(new BoardCoordinate(0, homeRank));

        if (!CanCastle(king, rook))
        {
            return;
        }

        if (!AreCastleSquaresEmpty(king, CastleSide.QueenSide))
        {
            return;
        }

        if (!AreCastleSquaresSafe(king, CastleSide.QueenSide))
        {
            return;
        }

        moves.Add(new MoveData(king, king.Coordinate, new BoardCoordinate(2, homeRank), null));
    }

    private void GenerateSlidingRule(Piece piece, MovementRule rule, List<MoveData> moves)
    {
        int maxDistance = rule.UnlimitedRange ? Mathf.Max(boardManager.BoardWidth, boardManager.BoardHeight) : rule.MaxDistance;

        for (int distance = 1; distance <= maxDistance; distance++)
        {
            BoardCoordinate target = new BoardCoordinate(
                piece.Coordinate.X + rule.Direction.x * distance, 
                piece.Coordinate.Y + rule.Direction.y * distance);

            if (!EvaluateSlidingSquare(piece, target, moves))
            {
                break;
            }
        }
    }

    private bool EvaluateSlidingSquare(Piece piece, BoardCoordinate target, List<MoveData> moves)
    {
        if (!boardManager.IsWithinBounds(target))
        {
            return false;
        }

        Piece targetPiece = boardManager.GetPiece(target);

        if (targetPiece == null)
        {
            moves.Add(new MoveData(piece, piece.Coordinate, target, null));

            return true;
        }

        if (targetPiece.Team != piece.Team)
        {
            moves.Add(new MoveData(piece, piece.Coordinate, target, targetPiece));
        }

        return false;
    }

    private void EvaluateJumpSquare(Piece piece, BoardCoordinate target, List<MoveData> moves)
    {
        if (!boardManager.IsWithinBounds(target))
        {
            return;
        }

        Piece targetPiece = boardManager.GetPiece(target);

        if (targetPiece == null)
        {
            moves.Add(new MoveData(piece, piece.Coordinate, target, null));

            return;
        }

        if (targetPiece.Team != piece.Team)
        {
            moves.Add(new MoveData(piece, piece.Coordinate, target, targetPiece));
        }
    }

    private void GeneratePawnAdvance(Piece piece, List<MoveData> moves)
    {
        int direction = piece.Team == Team.White ? 1 : -1;

        BoardCoordinate target = new BoardCoordinate(piece.Coordinate.X, piece.Coordinate.Y + direction);

        if (!boardManager.IsWithinBounds(target))
        {
            return;
        }

        if (boardManager.GetPiece(target) != null)
        {
            return;
        }

        moves.Add(new MoveData(piece, piece.Coordinate, target, null));
    }

    private void GeneratePawnDoubleAdvance(Piece piece, List<MoveData> moves)
    {
        if (piece.HasMoved)
        {
            return;
        }

        int direction = piece.Team == Team.White ? 1 : -1;

        BoardCoordinate intermediate = new BoardCoordinate(piece.Coordinate.X, piece.Coordinate.Y + direction);

        BoardCoordinate target = new BoardCoordinate(piece.Coordinate.X, piece.Coordinate.Y + direction * 2);

        if (!boardManager.IsWithinBounds(target))
        {
            return;
        }

        if (boardManager.GetPiece(intermediate) != null)
        {
            return;
        }

        if (boardManager.GetPiece(target) != null)
        {
            return;
        }

        moves.Add(new MoveData(piece, piece.Coordinate, target, null));
    }

    private void GeneratePawnCaptures(Piece piece, List<MoveData> moves)
    {
        int direction = piece.Team == Team.White ? 1 : - 1;

        EvaluatePawnCapture(piece, new BoardCoordinate(piece.Coordinate.X - 1, piece.Coordinate.Y + direction), moves);

        EvaluatePawnCapture(piece, new BoardCoordinate(piece.Coordinate.X + 1, piece.Coordinate.Y + direction), moves);
    }

    private void EvaluatePawnCapture(Piece piece, BoardCoordinate target, List<MoveData> moves)
    {
        if (!boardManager.IsWithinBounds(target))
        {
            return;
        }

        Piece targetPiece = boardManager.GetPiece(target);

        if (targetPiece == null)
        {
            return;
        }

        if (targetPiece.Team == piece.Team)
        {
            return;
        }

        moves.Add(new MoveData(piece, piece.Coordinate, target, targetPiece));
    }

    private void GeneratePawnAttackSquares(Piece piece, List<MoveData> moves)
    {
        int direction = piece.Team == Team.White ? 1 : -1;

        AddPawnAttackSquare(piece, new BoardCoordinate(piece.Coordinate.X - 1, piece.Coordinate.Y + direction), moves);

        AddPawnAttackSquare(piece, new BoardCoordinate(piece.Coordinate.X + 1, piece.Coordinate.Y + direction), moves);
    }

    // Private State

    // Private Validation

    private bool IsSquareAttacked(BoardCoordinate coordinate, Team attackingTeam)
    {
        foreach (Piece piece in boardManager.GetActivePieces())
        {
            if (piece.Team != attackingTeam)
            {
                continue;
            }

            List<MoveData> moves = GenerateMoves(piece, MoveGenerationMode.Attack);

            foreach (MoveData move in moves)
            {
                if (move.TargetCoordinate == coordinate)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Private Helpers

    private void AddPawnAttackSquare(Piece piece, BoardCoordinate target, List<MoveData> moves)
    {
        if (!boardManager.IsWithinBounds(target))
        {
            return;
        }

        Piece targetPiece = boardManager.GetPiece(target);

        moves.Add(new MoveData(piece, piece.Coordinate, target, targetPiece));
    }

    private bool CanCastle(Piece king, Piece rook)
    {
        if (king == null || rook == null)
        {
            return false;
        }

        if (king.HasMoved)
        {
            return false;
        }

        if (rook.HasMoved)
        {
            return false;
        }

        if (rook.PieceData.PieceType != PieceType.Rook)
        {
            return false;
        }

        if (rook.Team != king.Team)
        {
            return false;
        }

        return true;
    }

    private bool AreCastleSquaresEmpty(Piece king, CastleSide castleSide)
    {
        int homeRank = king.Coordinate.Y;

        int[] files = castleSide == CastleSide.KingSide ? new[] { 5, 6 } : new[] { 1, 2, 3 };

        foreach (int file in files)
        {
            if (boardManager.GetPiece(new BoardCoordinate(file, homeRank)) != null)
            {
                return false;
            }
        }

        return true;
    }

    private bool AreCastleSquaresSafe(Piece king, CastleSide castleSide)
    {
        Team attackingTeam = GetOpposingTeam(king.Team);

        int direction = castleSide == CastleSide.KingSide ? 1 : -1;

        BoardCoordinate currentSquare = king.Coordinate;

        BoardCoordinate transitSquare = new(currentSquare.X + direction, currentSquare.Y);

        BoardCoordinate destinationSquare = new(currentSquare.X + (2 * direction), currentSquare.Y);

        if (IsSquareAttacked(currentSquare, attackingTeam))
        {
            return false;
        }

        if (IsSquareAttacked(transitSquare, attackingTeam))
        {
            return false;
        }

        if (IsSquareAttacked(destinationSquare, attackingTeam))
        {
            return false;
        }

        return true;
    }

    private Team GetOpposingTeam(Team team)
    {
        return team == Team.White ? Team.Black : Team.White;
    }

    private bool TeamHasLegalMove(Team team)
    {
        List<Piece> pieces = new(boardManager.GetActivePieces());

        foreach (Piece piece in pieces)
        {
            if (piece.Team != team)
            {
                continue;
            }

            if (GetLegalMoves(piece).Count > 0)
            {
                return true;
            }
        }

        return false;
    }
}