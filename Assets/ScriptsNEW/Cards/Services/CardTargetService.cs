public class CardTargetService
{
    // Public Methods

    public bool IsValidTarget(Card card, CardTarget target)
    {
        if (card == null)
        {
            return false;
        }

        if (target == null)
        {
            return card.CardData.TargetType == TargetType.None;
        }

        return target.TargetType == card.CardData.TargetType;
    }
}