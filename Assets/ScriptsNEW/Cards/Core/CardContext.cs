public class CardContext
{
    // Public Properties

    public Card Card { get; }

    public CardTarget Target { get; }

    public Team Owner { get; }

    public BoardManager BoardManager { get; }

    public GameManager GameManager { get; }

    public CardManager CardManager { get; }

    // Constructors

    public CardContext(Card card, CardTarget target, Team owner, BoardManager boardManager, GameManager gameManager, CardManager cardManager )
    {
        Card = card;
        Target = target;
        Owner = owner;
        BoardManager = boardManager;
        GameManager = gameManager;
        CardManager = cardManager;
    }
}