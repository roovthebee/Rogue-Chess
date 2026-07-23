public class TileTarget : CardTarget
{
    // Public Properties

    public override TargetType TargetType => TargetType.Tile;

    public BoardCoordinate Coordinate { get; }

    // Constructors

    public TileTarget(BoardCoordinate coordinate)
    {
        Coordinate = coordinate;
    }
}