using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private PieceSpawner pieceSpawner;

    [SerializeField] private int boardWidth = 8;

    [SerializeField] private int boardHeight = 8;

    [SerializeField] private float tileSize = 1f;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Color lightTileColor;

    [SerializeField] private Color darkTileColor;

    // Private Fields

    private Tile[,] tiles;

    private Piece[,] occupiedPieces;

    private readonly List<Piece> activePieces = new();

    private Tile checkTile;

    // Public Properties

    public int BoardWidth => boardWidth;

    public int BoardHeight => boardHeight;

    // Events

    public static event Action<MoveData> PieceMoved;

    // Unity Messages

    private void Awake()
    {
        GenerateBoard();
    }

    // Public Methods

    public void GenerateBoard()
    {
        tiles = new Tile[boardWidth, boardHeight];
        occupiedPieces = new Piece[boardWidth, boardHeight];

        CreateTiles();
    }

    public Tile GetTile(BoardCoordinate coordinate)
    {
        if (!IsWithinBounds(coordinate))
        {
            return null;
        }

        return tiles[coordinate.X, coordinate.Y];
    }

    public Piece GetPiece(BoardCoordinate coordinate)
    {
        if (!IsWithinBounds(coordinate))
        {
            return null;
        }

        return occupiedPieces[coordinate.X, coordinate.Y];
    }

    public IReadOnlyList<Piece> GetActivePieces()
    {
        return activePieces;
    }

    public void PlacePiece(Piece piece, BoardCoordinate coordinate)
    {
        occupiedPieces[coordinate.X, coordinate.Y] = piece;

        activePieces.Add(piece);

        piece.SetCoordinate(coordinate);
    }

    public void RemovePiece(BoardCoordinate coordinate)
    {
        Piece piece = GetPiece(coordinate);

        if (piece == null)
        {
            return;
        }

        occupiedPieces[coordinate.X, coordinate.Y] = null;

        activePieces.Remove(piece);

        Destroy(piece.gameObject);
    }

    public void ExecuteMove(MoveData move)
    {
        if (move.IsCapture)
        {
            RemovePiece(move.TargetCoordinate);
        }

        MovePieceOnBoard(move.Piece, move.StartCoordinate, move.TargetCoordinate);

        if (move.IsCastle)
        {
            ApplyCastle(move);
        }

        CompleteMove(move);

        HandlePromotion(move.Piece);

        PieceMoved?.Invoke(move);
    }

    private void HandlePromotion(Piece piece)
    {
        if (piece.PieceData.PieceType != PieceType.Pawn)
        {
            return;
        }

        bool reachedPromotionRank = piece.Team == Team.White 
            ? piece.Coordinate.Y == boardHeight - 1 
            : piece.Coordinate.Y == 0;
        
        if (!reachedPromotionRank)
        {
            return;
        }

        PieceData promotionPiece = pieceSpawner.GetPromotionPiece(piece.Team);

        Team team = piece.Team;

        BoardCoordinate coordinate = piece.Coordinate;

        RemovePiece(coordinate);

        pieceSpawner.SpawnPiece(promotionPiece, team, coordinate);
    }

    public void ApplyMove(MoveData move)
    {
        if (move.IsCapture)
        {
            occupiedPieces[move.TargetCoordinate.X, move.TargetCoordinate.Y] = null;

            activePieces.Remove(move.CapturedPiece);
        }

        MovePieceOnBoard(move.Piece, move.StartCoordinate, move.TargetCoordinate);

        if (move.IsCastle)
        {
            ApplyCastle(move);
        }
    }

    public void RevertMove(MoveData move)
    {
        if (move.IsCastle)
        {
            RevertCastle(move);    
        }

        MovePieceOnBoard(move.Piece, move.TargetCoordinate, move.StartCoordinate);

        if (move.IsCapture)
        {
            occupiedPieces[move.TargetCoordinate.X, move.TargetCoordinate.Y] = move.CapturedPiece;

            move.CapturedPiece.SetCoordinate(move.TargetCoordinate);

            activePieces.Add(move.CapturedPiece);
        }
    }

    public Vector3 GetWorldPosition(BoardCoordinate coordinate)
    {
        float xOffset = (boardWidth - 1) * tileSize * 0.5f;
        float yOffset = (boardHeight - 1) * tileSize * 0.5f;

        return new Vector3(coordinate.X * tileSize - xOffset, coordinate.Y * tileSize - yOffset, 0f);
    }

    public bool IsWithinBounds(BoardCoordinate coordinate)
    {
        return coordinate.X >= 0 && coordinate.X < boardWidth && coordinate.Y >= 0 && coordinate.Y < boardHeight;
    }

    public void ShowCheck(BoardCoordinate coordinate)
    {
        ClearCheck();

        checkTile = GetTile(coordinate);

        checkTile.HighlightCheck();
    }

    public void ClearCheck()
    {
        if (checkTile == null)
        {
            return;
        }

        checkTile.ResetCheckHighlight();

        checkTile = null;
    }

    // Private Workflow

    private void CreateTiles()
    {
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                CreateTile(new BoardCoordinate(x, y));
            }
        }
    }

    private void CreateTile(BoardCoordinate coordinate)
    {
        Tile tile = Instantiate(tilePrefab, GetWorldPosition(coordinate), Quaternion.identity, transform);

        tile.Initialize(coordinate);

        tile.SetColor(GetTileColor(coordinate));

        tiles[coordinate.X, coordinate.Y] = tile;
    }

    private void CompleteMove(MoveData move)
    {
        move.Piece.transform.position = GetWorldPosition(move.TargetCoordinate);

        move.Piece.MarkAsMoved();
    }

    private void ApplyCastle(MoveData move)
    {
        bool kingSide = move.TargetCoordinate.X > move.StartCoordinate.X;

        int homeRank = move.StartCoordinate.Y;

        BoardCoordinate rookStart = kingSide ? new BoardCoordinate(7, homeRank) : new BoardCoordinate(0, homeRank);

        BoardCoordinate rookTarget = kingSide ? new BoardCoordinate(5, homeRank) : new BoardCoordinate(3, homeRank);

        Piece rook = GetPiece(rookStart);

        MovePieceOnBoard(rook, rookStart, rookTarget);

        rook.transform.position = GetWorldPosition(rookTarget);
    }

    private void RevertCastle(MoveData move)
    {
        bool kingSide = move.TargetCoordinate.X > move.StartCoordinate.X;

        int homeRank = move.StartCoordinate.Y;

        BoardCoordinate rookStart = kingSide ? new BoardCoordinate(5, homeRank) : new BoardCoordinate(3, homeRank);

        BoardCoordinate rookTarget = kingSide ? new BoardCoordinate(7, homeRank) : new BoardCoordinate(0, homeRank);

        Piece rook = GetPiece(rookStart);

        MovePieceOnBoard(rook, rookStart, rookTarget);

        rook.transform.position = GetWorldPosition(rookTarget);
    }

    // Private State

    private Color GetTileColor(BoardCoordinate coordinate)
    {
        bool isLightTile = (coordinate.X + coordinate.Y) % 2 != 0;

        return isLightTile ? lightTileColor : darkTileColor;
    }

    private void UpdatePiecePosition(Piece piece, BoardCoordinate startCoordinate, BoardCoordinate targetCoordinate)
    {
        occupiedPieces[startCoordinate.X, startCoordinate.Y] = null;

        occupiedPieces[targetCoordinate.X, targetCoordinate.Y] = piece;
    }

    private void MovePieceOnBoard(Piece piece, BoardCoordinate startCoordinate, BoardCoordinate targetCoordinate)
    {
        UpdatePiecePosition(piece, startCoordinate, targetCoordinate);

        piece.SetCoordinate(targetCoordinate);
    }

    // Private Validation

    // Private Helpers

}