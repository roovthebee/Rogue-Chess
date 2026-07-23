using UnityEngine;

public class Piece : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private PieceData pieceData;

    // Private Fields

    private Team team;

    private BoardCoordinate coordinate;

    private bool hasMoved;

    private SpriteRenderer spriteRenderer;

    // Public Properties

    public PieceData PieceData => pieceData;

    public Team Team => team;

    public BoardCoordinate Coordinate => coordinate;

    public bool HasMoved => hasMoved;

    // Unity Messages

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Public Methods

    public void Initialize(PieceData pieceData, Team team, BoardCoordinate coordinate)
    {
        this.pieceData = pieceData;
        this.team = team;
        hasMoved = false;

        spriteRenderer.sprite = pieceData.Sprite;

        SetCoordinate(coordinate);
    }

    public void SetCoordinate(BoardCoordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public void MarkAsMoved()
    {
        hasMoved = true;
    }
}