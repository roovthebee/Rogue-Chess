using System;

public readonly struct MoveData
{
    // Public Properties

    public Piece Piece { get; }

    public BoardCoordinate StartCoordinate { get; }

    public BoardCoordinate TargetCoordinate { get; }

    public Piece CapturedPiece { get; }

    public bool IsCapture => CapturedPiece != null;

    public bool IsCastle => Piece.PieceData.PieceType == PieceType.King && Math.Abs(TargetCoordinate.X - StartCoordinate.X) == 2;

    // Constructors

    public MoveData(Piece piece, BoardCoordinate startCoordinate, BoardCoordinate targetCoordinate, Piece capturedPiece)
    {
        Piece = piece;

        StartCoordinate = startCoordinate;

        TargetCoordinate = targetCoordinate;

        CapturedPiece = capturedPiece;
    }
}