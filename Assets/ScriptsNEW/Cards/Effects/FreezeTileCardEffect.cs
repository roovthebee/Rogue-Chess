using UnityEngine;

[CreateAssetMenu(menuName = "Rogue Chess/Cards/Effects/Freeze Tile")]
public class FreezeTileCardEffect : CardEffect
{
    // Serialized Fields

    [SerializeField] private FrozenTileEffect freezeTileEffect;

    [SerializeField] private int duration = 5;

    // Public Methods

    public override bool Resolve(CardContext context)
    {
        if (context.Target is not TileTarget tileTarget)
        {
            return false;
        }

        BoardCoordinate coordinate = tileTarget.Coordinate;

        Tile tile = context.BoardManager.GetTile(coordinate);

        if (tile == null)
        {
            return false;
        }

        if (tile.TemporaryEffects.Count > 0)
        {
            return false;
        }

        if (context.BoardManager.GetPiece(coordinate))
        {
            return false;
        }

        tile.AddTemporaryEffect(freezeTileEffect, duration);

        return true;
    }
}