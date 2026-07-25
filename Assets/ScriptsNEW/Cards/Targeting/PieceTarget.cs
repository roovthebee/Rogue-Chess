public class PieceTarget : CardTarget
{
    // Public Properties

    public override TargetType TargetType => TargetType.Piece;

    public Piece Piece { get; }

    // Constructors

    public PieceTarget(Piece piece)
    {
        Piece = piece;
    }
}