using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rogue Chess/Cards/Effects/Temporary Moves")]
public class TemporaryMovesCardEffect : CardEffect
{
    // Serialized Fields

    [SerializeField] List<MovementRule> temporaryMoves;

    [SerializeField] private int duration = 2;

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

        if (piece.Team != context.Owner)
        {
            return false;
        }

        if (piece.PieceData.PieceType == PieceType.Knight)
        {
            return false;
        }

        foreach (MovementRule rule in temporaryMoves)
        {
            piece.AddTemporaryMovementRule(rule, duration);
        }

        return true;
    }
}