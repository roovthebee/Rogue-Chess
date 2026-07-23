using UnityEngine;

[CreateAssetMenu(menuName = "Rogue Chess/Cards/Effects/Destroy Piece")]
public class DestroyPieceEffect : CardEffect
{
    // Serialized Fields

    [SerializeField] private PieceType targetPieceType;

    // Public Methods

    public override bool Resolve(CardContext context)
    {
        if (context.Target is not PieceTarget pieceTarget)
        {
            return false;
        }

        Piece piece = pieceTarget.Piece;

        if (piece == null)
        {
            return false;
        }

        if (piece.Team == context.Owner)
        {
            return false;
        }

        if (piece.PieceData.PieceType != targetPieceType)
        {
            return false;
        }

        context.BoardManager.RemovePiece(piece.Coordinate);

        return true;
    }
}