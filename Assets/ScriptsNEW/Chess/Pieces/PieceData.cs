using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rogue Chess/Piece")]
public class PieceData : ScriptableObject
{
    // Serialized Fields

    [SerializeField] private string pieceName;

    [SerializeField] private Sprite sprite;

    [SerializeField] private PieceType pieceType;

    [SerializeField] private List<MovementRule> movementRules = new();

    // Public Properties

    public string PieceName => pieceName;

    public Sprite Sprite => sprite;

    public PieceType PieceType => pieceType;

    public IReadOnlyList<MovementRule> MovementRules => movementRules;
}