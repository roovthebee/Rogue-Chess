using UnityEngine;

[CreateAssetMenu(menuName = "Rogue Chess/Tile Effects/Freeze Tile")]
public class FrozenTileEffect : TileEffect
{
    public override void Apply(Tile tile, BoardManager boardManager)
    {
        Piece piece = boardManager.GetPiece(tile.Coordinate);

        if (piece == null)
        {
            return;
        }

        if (piece.HasStatus(PieceStatus.Frozen))
        {
            return;
        }

        piece.AddTemporaryStatus(PieceStatus.Frozen, 4);
    }

    public override void OnRemove(Tile tile, BoardManager boardManager)
    {
        Piece piece = boardManager.GetPiece(tile.Coordinate);

        if (piece == null)
        {
            return;
        }

        if (piece.HasStatus(PieceStatus.Frozen))
        {
            piece.RemoveTemporaryStatus(PieceStatus.Frozen);
        }
    }
}