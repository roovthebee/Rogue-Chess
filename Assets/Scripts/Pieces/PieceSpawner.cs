using RogueChess.Board;
using UnityEngine;

namespace RogueChess.Pieces
{
    public class PieceSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private Piece piecePrefab;

        [Header("White Piece Data")]
        [SerializeField] private PieceData whiteKingData;
        [SerializeField] private PieceData whiteRookData;
        [SerializeField] private PieceData whiteBishopData;
        [SerializeField] private PieceData whiteQueenData;
        [SerializeField] private PieceData whiteKnightData;
        [SerializeField] private PieceData whitePawnData;

        [Header("Black Piece Data")]
        [SerializeField] private PieceData blackKingData;
        [SerializeField] private PieceData blackRookData;
        [SerializeField] private PieceData blackBishopData;
        [SerializeField] private PieceData blackQueenData;
        [SerializeField] private PieceData blackKnightData;
        [SerializeField] private PieceData blackPawnData;

        private void Start()
        {
            boardManager.InitializeBoard();

            SpawnInitialPieces();
        }

        private void SpawnInitialPieces()
        {
            SpawnPiece(whiteKingData, Team.White, new BoardCoordinate(4, 0));
            SpawnPiece(whiteRookData, Team.White, new BoardCoordinate(0, 0));
            SpawnPiece(whiteRookData, Team.White, new BoardCoordinate(7, 0));
            SpawnPiece(whiteBishopData, Team.White, new BoardCoordinate(2, 0));
            SpawnPiece(whiteBishopData, Team.White, new BoardCoordinate(5, 0));
            SpawnPiece(whiteQueenData, Team.White, new BoardCoordinate(3, 0));
            SpawnPiece(whiteKnightData, Team.White, new BoardCoordinate(1, 0));
            SpawnPiece(whiteKnightData, Team.White, new BoardCoordinate(6, 0));

            SpawnPiece(blackKingData, Team.Black, new BoardCoordinate(4, 7));
            SpawnPiece(blackRookData, Team.Black, new BoardCoordinate(0, 7));
            SpawnPiece(blackRookData, Team.Black, new BoardCoordinate(7, 7));
            SpawnPiece(blackBishopData, Team.Black, new BoardCoordinate(2, 7));
            SpawnPiece(blackBishopData, Team.Black, new BoardCoordinate(5, 7));
            SpawnPiece(blackQueenData, Team.Black, new BoardCoordinate(3, 7));
            SpawnPiece(blackKnightData, Team.Black, new BoardCoordinate(1, 7));
            SpawnPiece(blackKnightData, Team.Black, new BoardCoordinate(6, 7));

            for (int i = 0; i < 8; i++)
            {
                SpawnPiece(whitePawnData, Team.White, new BoardCoordinate(i, 1));
                SpawnPiece(blackPawnData, Team.Black, new BoardCoordinate(i, 6));
            }
        }

        public Piece SpawnPiece(PieceData pieceData, Team team, BoardCoordinate coordinate)
        {
            Vector3 worldPosition = boardManager.GetWorldPosition(coordinate);

            Piece spawnedPiece = Instantiate(piecePrefab, worldPosition, Quaternion.identity);
            spawnedPiece.Initialize(pieceData, team, coordinate);

            boardManager.PlacePiece(spawnedPiece, coordinate);

            spawnedPiece.name = $"{team} {pieceData.PieceName}";

            return spawnedPiece;
        }

        public PieceData GetDefaultPromotionPiece(Team team)
        {
            return team == Team.White ? whiteQueenData : blackQueenData;
        }
    }
}