using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private BoardManager boardManager;

    [SerializeField] private Piece piecePrefab;

    [Header("White Pieces")]

    [SerializeField] private PieceData whitePawn;
    [SerializeField] private PieceData whiteKnight;
    [SerializeField] private PieceData whiteBishop;
    [SerializeField] private PieceData whiteRook;
    [SerializeField] private PieceData whiteQueen;
    [SerializeField] private PieceData whiteKing;

    [Header("Black Pieces")]

    [SerializeField] private PieceData blackPawn;
    [SerializeField] private PieceData blackKnight;
    [SerializeField] private PieceData blackBishop;
    [SerializeField] private PieceData blackRook;
    [SerializeField] private PieceData blackQueen;
    [SerializeField] private PieceData blackKing;

    // Private Fields

    // Public Properties

    // Events

    // Unity Messages

    private void Start()
    {
        SpawnPieces();

        GameManager.Instance.StartGame();
    }

    // Public Methods

    public void SpawnPieces()
    {
        SpawnWhitePieces();
        SpawnBlackPieces();
    }

    public Piece SpawnPiece(PieceData pieceData, Team team, BoardCoordinate coordinate)
    {
        Piece piece = Instantiate(piecePrefab, boardManager.GetWorldPosition(coordinate), Quaternion.identity, transform);
        
        piece.UpdateView();

        piece.Initialize(pieceData, team, coordinate);

        boardManager.PlacePiece(piece, coordinate);

        return piece;
    }

    public PieceData GetPromotionPiece(Team team)
    {
        return team == Team.White ? whiteQueen : blackQueen;
    }

    // Private Workflow

    private void SpawnWhitePieces()
    {
        SpawnWhiteBackRank();
        SpawnWhitePawns();
    }

    private void SpawnBlackPieces()
    {
        SpawnBlackBackRank();
        SpawnBlackPawns();
    }

    private void SpawnWhitePawns()
    {
        for (int x = 0; x < boardManager.BoardWidth; x++)
        {
            SpawnPiece(whitePawn, Team.White, new BoardCoordinate(x, 1));
        }
    }

    private void SpawnBlackPawns()
    {
        for (int x = 0; x < boardManager.BoardWidth; x++)
        {
            SpawnPiece(blackPawn, Team.Black, new BoardCoordinate(x, 6));
        }
    }

    private void SpawnWhiteBackRank()
    {
        SpawnPiece(whiteRook,   Team.White, new BoardCoordinate(0, 0));
        SpawnPiece(whiteKnight, Team.White, new BoardCoordinate(1, 0));
        SpawnPiece(whiteBishop, Team.White, new BoardCoordinate(2, 0));
        SpawnPiece(whiteQueen,  Team.White, new BoardCoordinate(3, 0));
        SpawnPiece(whiteKing,   Team.White, new BoardCoordinate(4, 0));
        SpawnPiece(whiteBishop, Team.White, new BoardCoordinate(5, 0));
        SpawnPiece(whiteKnight, Team.White, new BoardCoordinate(6, 0));
        SpawnPiece(whiteRook,   Team.White, new BoardCoordinate(7, 0));
    }

    private void SpawnBlackBackRank()
    {
        SpawnPiece(blackRook,   Team.Black, new BoardCoordinate(0, 7));
        SpawnPiece(blackKnight, Team.Black, new BoardCoordinate(1, 7));
        SpawnPiece(blackBishop, Team.Black, new BoardCoordinate(2, 7));
        SpawnPiece(blackQueen,  Team.Black, new BoardCoordinate(3, 7));
        SpawnPiece(blackKing,   Team.Black, new BoardCoordinate(4, 7));
        SpawnPiece(blackBishop, Team.Black, new BoardCoordinate(5, 7));
        SpawnPiece(blackKnight, Team.Black, new BoardCoordinate(6, 7));
        SpawnPiece(blackRook,   Team.Black, new BoardCoordinate(7, 7));
    }
}