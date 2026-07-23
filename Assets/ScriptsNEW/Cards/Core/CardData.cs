using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rogue Chess/Cards/Card")]
public class CardData : ScriptableObject
{
    // Serialized Fields

    [SerializeField] private string cardName;

    [SerializeField, TextArea] private string description;

    [SerializeField] private CardRarity rarity;

    [SerializeField] private Sprite artwork;

    [SerializeField] private int cost = 1;

    [SerializeField] private TargetType targetType;

    [SerializeField] private List<CardEffect> effects = new();

    // Public Properties

    public string CardName => cardName;

    public string Description => description;

    public CardRarity Rarity => rarity;

    public Sprite Artwork => artwork;

    public int Cost => cost;

    public TargetType TargetType => targetType;

    public IReadOnlyList<CardEffect> Effects => effects;
}