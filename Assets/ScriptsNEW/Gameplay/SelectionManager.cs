using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private CardManager cardManager;

    [SerializeField] private BoardManager boardManager;

    [SerializeField] private ChessRules chessRules;

    [SerializeField] private NetworkGameManager networkGameManager;

    // Private Fields

    private Piece selectedPiece;

    private readonly List<MoveData> currentMoves = new();

    private readonly List<Tile> highlightedTiles = new();

    // Unity Messages

    private void OnEnable()
    {
        Tile.TileClicked += HandleTileClicked;
    }

    private void OnDisable()
    {
        Tile.TileClicked -= HandleTileClicked;
    }

    // Public Methods

    public void ClearSelection()
    {
        if (selectedPiece == null)
        {
            return;
        }

        ClearHighlights();

        selectedPiece = null;
        currentMoves.Clear();
    }

    // Private Workflow

    private void HandleTileClicked(Tile tile)
    {
        if (cardManager.IsTargeting)
        {
            HandleCardTarget(tile);
            return;
        }

        if (selectedPiece == null)
        {
            TrySelectPiece(tile);

            return;
        }

        if (TryMoveSelectedPiece(tile))
        {
            return;
        }

        TrySelectPiece(tile);
    }

    private void TrySelectPiece(Tile tile)
    {
        Piece piece = boardManager.GetPiece(tile.Coordinate);

        if (piece == null)
        {
            DeselectPiece();

            return;
        }

        if (piece.Team != GameManager.Instance.CurrentTurn)
        {
            DeselectPiece();

            return;
        }

        if (!PlayerRoleManager.Instance.CanControlTeam(piece.Team))
        {
            DeselectPiece();

            return;
        }

        SelectPiece(piece);
    }

    private bool TryMoveSelectedPiece(Tile tile)
    {
        foreach (MoveData move in currentMoves)
        {
            if (move.TargetCoordinate != tile.Coordinate)
            {
                continue;
            }

            networkGameManager.ExecuteMove(move);

            DeselectPiece();

            return true;
        }

        return false;
    }

    private void SelectPiece(Piece piece)
    {
        DeselectPiece();

        selectedPiece = piece;

        currentMoves.Clear();

        currentMoves.AddRange(chessRules.GetLegalMoves(piece));

        HighlightCurrentMoves();
    }

    private void DeselectPiece()
    {
        selectedPiece = null;

        currentMoves.Clear();

        ClearHighlights();
    }

    private void HighlightCurrentMoves()
    {
        foreach (MoveData move in currentMoves)
        {
            Tile tile = boardManager.GetTile(move.TargetCoordinate);

            tile.Highlight(move.IsCapture);

            highlightedTiles.Add(tile);
        }
    }

    private void ClearHighlights()
    {
        foreach (Tile tile in highlightedTiles)
        {
            tile.ResetHighlight();
        }

        highlightedTiles.Clear();
    }

    private void HandleCardTarget(Tile tile)
    {
        Card selectedCard = cardManager.SelectedCard;

        switch (selectedCard.CardData.TargetType)
        {
            case TargetType.Tile:
                cardManager.SelectTarget(new TileTarget(tile.Coordinate));
                return;

            case TargetType.Piece:

                Piece piece = boardManager.GetPiece(tile.Coordinate);

                if (piece == null)
                {
                    return;
                }

                cardManager.SelectTarget(new PieceTarget(piece));
                return;
        }
    }

    // Private State

    // Private Validation

    // Private Helpers
}