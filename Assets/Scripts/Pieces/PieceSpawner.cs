using RogueChess.Board;
using UnityEngine;

namespace RogueChess.Pieces
{
    public class PieceSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private Piece piecePrefab;

        [Header("Piece Data")]
        [SerializeField] private PieceData whiteKingData;
        [SerializeField] private PieceData blackKingData;

        private void Start()
        {
            boardManager.InitializeBoard();

            SpawnInitialPieces();
        }

        private void SpawnInitialPieces()
        {
            SpawnPiece(whiteKingData, Team.White, new BoardCoordinate(4, 0));
            SpawnPiece(blackKingData, Team.Black, new BoardCoordinate(4, 7));
        }

        private void SpawnPiece(PieceData pieceData, Team team, BoardCoordinate coordinate)
        {
            Vector3 worldPosition = boardManager.GetWorldPosition(coordinate);

            Piece spawnedPiece = Instantiate(piecePrefab, worldPosition, Quaternion.identity);
            spawnedPiece.Initialize(pieceData, team, coordinate);

            boardManager.PlacePiece(spawnedPiece, coordinate);

            spawnedPiece.name = $"{team} {pieceData.PieceName}";
        }
    }
}