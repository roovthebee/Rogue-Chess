using RogueChess.Board;
using RogueChess.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RogueChess.Pieces
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;

        private List<MoveData> currentMoves = new List<MoveData>();
        private readonly List<Tile> highlightedTiles = new List<Tile>();

        private Piece selectedPiece;
        public Piece SelectedPiece => selectedPiece;

        private void OnEnable()
        {
            Tile.OnTileClicked += HandleTileClicked;
        }

        private void OnDisable()
        {
            Tile.OnTileClicked -= HandleTileClicked;
        }

        private void HandleTileClicked(Tile tile)
        {
            if (GameManager.Instance.IsGameOver)
            {
                return;
            }

            if (selectedPiece == null)
            {
                Piece piece = boardManager.GetPieceAtCoordinate(tile.Coordinate);

                if (piece != null)
                {
                    SelectPiece(piece);
                }

                return;
            }

            if (IsValidMove(tile.Coordinate))
            {
                MovePiece(tile.Coordinate);
            }
            else
            {
                DeselectPiece();
            }
        }

        private void SelectPiece(Piece piece)
        {
            if (piece.Team != GameManager.Instance.CurrentTurn)
            {
                return;
            }

            ClearHighlights();

            selectedPiece = piece;
            currentMoves = PieceMovement.GetMoves(piece, boardManager);

            HighlightMoves();
        }

        private void ClearHighlights()
        {
            foreach (Tile tile in highlightedTiles)
            {
                tile.HideMoveIndicator();
                tile.HideCaptureIndicator();
            }

            highlightedTiles.Clear();
        }

        private void HighlightMoves()
        {
            foreach (MoveData move in currentMoves)
            {
                Tile tile = boardManager.GetTile(move.TargetCoordinate);
                
                if (tile == null)
                {
                    continue;
                }

                if (move.IsCapture)
                {
                    tile.ShowCaptureIndicator();
                }
                else
                {
                    tile.ShowMoveIndicator();
                }

                highlightedTiles.Add(tile);
            }
        }

        private bool IsValidMove(BoardCoordinate coordinate)
        {
            foreach (MoveData move in currentMoves)
            {
                if (move.TargetCoordinate.X == coordinate.X && move.TargetCoordinate.Y == coordinate.Y)
                {
                    return true;
                }
            }

            return false;
        }

        private void DeselectPiece()
        {
            ClearHighlights();
            selectedPiece = null;
            currentMoves.Clear();
        }

        private void MovePiece(BoardCoordinate destination)
        {
            BoardCoordinate previousCoordinate = selectedPiece.Coordinate;

            Piece targetPiece = boardManager.GetPieceAtCoordinate(destination);

            if (targetPiece != null)
            {
                if (targetPiece.PieceData.PieceType == PieceType.King)
                {
                    GameManager.Instance.EndGame(selectedPiece.Team);
                }

                Destroy(targetPiece.gameObject);
                boardManager.RemovePiece(destination);
            }

            boardManager.RemovePiece(previousCoordinate);
            boardManager.PlacePiece(selectedPiece, destination);
            selectedPiece.SetCoordinate(destination);

            selectedPiece.transform.position = boardManager.GetWorldPosition(destination);

            DeselectPiece();

            GameManager.Instance.EndTurn();
        }
    }
}