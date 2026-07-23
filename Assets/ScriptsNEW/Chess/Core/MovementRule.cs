using UnityEngine;

[CreateAssetMenu(menuName = "Rogue Chess/Movement Rule")]
public class MovementRule : ScriptableObject
{
    // Serialized Fields

    [SerializeField] private Vector2Int direction;

    [SerializeField] private int maxDistance = 1;

    [SerializeField] private bool unlimitedRange;

    // Public Properties

    public Vector2Int Direction => direction;

    public int MaxDistance => maxDistance;

    public bool UnlimitedRange => unlimitedRange;
}