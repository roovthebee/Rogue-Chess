public class TemporaryTileEffect
{
    // Public Properties

    public TileEffect Effect { get; }

    public int RemainingTurns { get; set; }

    // Constructor

    public TemporaryTileEffect(TileEffect effect, int remainingTurns)
    {
        Effect = effect;
        RemainingTurns = remainingTurns;
    }
}