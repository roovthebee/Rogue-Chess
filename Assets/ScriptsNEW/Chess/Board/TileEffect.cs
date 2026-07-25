using UnityEngine;

public abstract class TileEffect : ScriptableObject
{
    public abstract void Apply(Tile tile, BoardManager boardManager);

    public abstract void OnRemove(Tile tile, BoardManager boardManager);
}