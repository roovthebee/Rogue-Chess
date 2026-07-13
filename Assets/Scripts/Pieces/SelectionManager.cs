using RogueChess.Board;
using RogueChess.Core;
using RogueChess.Core.Rules;
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

        [SerializeField] private PieceSpawner pieceSpawner;

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

            MoveData? move = GetMove(tile.Coordinate);

            if (move.HasValue)
            {
                MovePiece(move.Value);
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
            currentMoves = ChessRules.GetLegalMoves(piece, boardManager);

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

        private MoveData? GetMove(BoardCoordinate coordinate)
        {
            foreach (MoveData move in currentMoves)
            {
                if (move.TargetCoordinate.X == coordinate.X && move.TargetCoordinate.Y == coordinate.Y)
                {
                    return move;
                }
            }

            return null;
        }

        private void DeselectPiece()
        {
            ClearHighlights();
            selectedPiece = null;
            currentMoves.Clear();
        }

        private void MovePiece(MoveData move)
        {
            switch (move.SpecialMoveType)
            {
                case SpecialMoveType.CastleKingSide:
                    ExecuteCastleKingSide(move);
                    return;

                case SpecialMoveType.CastleQueenSide:
                    ExecuteCastleQueenSide(move);
                    return;
            }

            BoardCoordinate previousCoordinate = selectedPiece.Coordinate;
            BoardCoordinate destination = move.TargetCoordinate;

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

            RelocatePiece(selectedPiece, previousCoordinate, destination);
            HandlePromotion(selectedPiece);
            DeselectPiece();

            GameManager.Instance.EndTurn();
        }

        private void ExecuteCastleKingSide(MoveData move)
        {
            BoardCoordinate kingStart = selectedPiece.Coordinate;
            BoardCoordinate rookStart = new BoardCoordinate(boardManager.BoardWidth - 1, kingStart.Y);

            Piece rook = boardManager.GetPieceAtCoordinate(rookStart);

            if (rook == null)
            {
                return;
            }

            BoardCoordinate kingDestination = move.TargetCoordinate;
            BoardCoordinate rookDestination = new BoardCoordinate(kingDestination.X - 1, kingDestination.Y);

            RelocatePiece(selectedPiece, kingStart, kingDestination);
            RelocatePiece(rook, rookStart, rookDestination);

            DeselectPiece();

            GameManager.Instance.EndTurn();
        }

        private void ExecuteCastleQueenSide(MoveData move)
        {
            BoardCoordinate kingStart = selectedPiece.Coordinate;
            BoardCoordinate rookStart = new BoardCoordinate(0, kingStart.Y);

            Piece rook = boardManager.GetPieceAtCoordinate(rookStart);

            if (rook == null)
            {
                return;
            }

            BoardCoordinate kingDestination = move.TargetCoordinate;
            BoardCoordinate rookDestination = new BoardCoordinate(kingDestination.X + 1, kingDestination.Y);

            RelocatePiece(selectedPiece, kingStart, kingDestination);
            RelocatePiece(rook, rookStart, rookDestination);

            DeselectPiece();

            GameManager.Instance.EndTurn();
        }

        private void RelocatePiece(Piece piece, BoardCoordinate from, BoardCoordinate to)
        {
            boardManager.RemovePiece(from);
            boardManager.PlacePiece(piece, to);
            piece.SetCoordinate(to);
            piece.MarkMoved();
            piece.transform.position = boardManager.GetWorldPosition(to);
        }

        private void HandlePromotion(Piece piece)
        {
            if (piece.PieceData.PieceType != PieceType.Pawn)
            {
                return;
            }

            int promotionRank = piece.Team == Team.White ? boardManager.BoardHeight - 1 : 0;

            if (piece.Coordinate.Y != promotionRank)
            {
                return;
            }

            PromotePawn(piece);
        }

        private void PromotePawn(Piece pawn)
        {
            BoardCoordinate coordinate = pawn.Coordinate;
            Team team = pawn.Team;

            PieceData promotionData = pieceSpawner.GetDefaultPromotionPiece(team);

            boardManager.RemovePiece(coordinate);

            if (selectedPiece == pawn)
            {
                selectedPiece = null;
            }

            Destroy(pawn.gameObject);

            pieceSpawner.SpawnPiece(promotionData, team, coordinate);
        }
    }
}