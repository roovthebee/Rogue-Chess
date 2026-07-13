using RogueChess.Board;
using RogueChess.Core.Rules;
using RogueChess.Pieces;
using System.Collections.Generic;
using UnityEngine;

namespace RogueChess.UI
{
    public class CheckIndicatorManager : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;

        private readonly List<Tile> highlightedTiles = new List<Tile>();
        private readonly Color checkColor = new Color(0.8f, 0.3f, 0.3f);

        public void Refresh()
        {
            ClearHighlights();

            HighlightKingIfChecked(Team.White);
            HighlightKingIfChecked(Team.Black);
        }

        private void ClearHighlights()
        {
            foreach (Tile tile in highlightedTiles)
            {
                tile.ResetColor();
            }

            highlightedTiles.Clear();
        }

        private void HighlightKingIfChecked(Team team)
        {
            if (!ChessRules.IsKingInCheck(team, boardManager))
            {
                return;
            }

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

                Tile tile = boardManager.GetTile(piece.Coordinate);

                if (tile == null)
                {
                    return;
                }

                tile.SetTemporaryColor(checkColor);
                highlightedTiles.Add(tile);

                return;
            }
        }
    }
}