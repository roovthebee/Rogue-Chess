public class TemporaryMovementRule
{
    // Public Properties

    public MovementRule Rule;

    public int RemainingTurns;

    // Constructors

    public TemporaryMovementRule(MovementRule rule, int remainingTurns)
    {
        Rule = rule;
        RemainingTurns = remainingTurns;
    }
}