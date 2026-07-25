public class TemporaryPieceStatus
{
    // Public Properties

    public PieceStatus Status { get; }

    public int RemainingTurns { get; set; }

    // Constructor

    public TemporaryPieceStatus(PieceStatus status, int remainingTurns)
    {
        Status = status;
        RemainingTurns = remainingTurns;
    }
}