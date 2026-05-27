using RogueChess.Board;
using RogueChess.Pieces;
using UnityEngine;

namespace RogueChess.Board
{
    public class BoardManager : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private int boardWidth = 8;
        [SerializeField] private int boardHeight = 8;
        [SerializeField] private float tileSize = 1f;

        [Header("References")]
        [SerializeField] private Tile tilePrefab;

        [Header("Tile Colors")]
        [SerializeField] private Color lightTileColor = Color.white;
        [SerializeField] private Color darkTileColor = Color.gray;

        private Tile[,] tiles;
        private Piece[,] occupiedPieces;

        public void InitializeBoard()
        {
            GenerateBoard();
        }

        private void GenerateBoard()
        {
            tiles = new Tile[boardWidth, boardHeight];
            occupiedPieces = new Piece[boardWidth, boardHeight];

            float boardOffsetX = (boardWidth - 1) / 2f;
            float boardOffsetY = (boardHeight - 1) / 2f;

            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Vector3 worldPosition = new Vector3((x - boardOffsetX) * tileSize, (y - boardOffsetY) * tileSize, 0f);
                    Tile spawnedTile = Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);

                    BoardCoordinate coordinate = new BoardCoordinate(x, y);
                    spawnedTile.Initialize(coordinate);

                    bool isLightTile = (x + y) % 2 != 0;
                    spawnedTile.SetColor(isLightTile ? lightTileColor : darkTileColor);
                    spawnedTile.name = $"Tile ({x}, {y})";

                    tiles[x, y] = spawnedTile;
                }
            }
        }

        public bool IsWithinBounds(BoardCoordinate coordinate)
        {
            return coordinate.X >= 0 && coordinate.X < boardWidth && coordinate.Y >= 0 && coordinate.Y < boardHeight;
        }

        public Tile GetTile(BoardCoordinate coordinate)
        {
            if (!IsWithinBounds(coordinate))
            {
                return null;
            }

            return tiles[coordinate.X, coordinate.Y];
        }

        public Vector3 GetWorldPosition(BoardCoordinate coordinate)
        {
            float boardOffsetX = (boardWidth - 1) / 2f;
            float boardOffsetY = (boardHeight - 1) / 2f;

            return new Vector3((coordinate.X - boardOffsetX) * tileSize, (coordinate.Y - boardOffsetY) * tileSize);
        }

        public bool IsTileOccupied(BoardCoordinate coordinate)
        {
            if (!IsWithinBounds(coordinate))
            {
                return false;
            }

            return occupiedPieces[coordinate.X, coordinate.Y] != null;
        }

        public Piece GetPieceAtCoordinate(BoardCoordinate coordinate)
        {
            if (!IsWithinBounds(coordinate))
            {
                return null;
            }

            return occupiedPieces[coordinate.X, coordinate.Y];
        }

        public void PlacePiece(Piece piece, BoardCoordinate coordinate)
        {
            if (!IsWithinBounds(coordinate))
            {
                return;
            }

            Debug.Log(piece);

            occupiedPieces[coordinate.X, coordinate.Y] = piece;
        }

        public void RemovePiece(BoardCoordinate coordinate)
        {
            if (!IsWithinBounds(coordinate))
            {
                return;
            }

            occupiedPieces[coordinate.X, coordinate.Y] = null;
        }
    }
}