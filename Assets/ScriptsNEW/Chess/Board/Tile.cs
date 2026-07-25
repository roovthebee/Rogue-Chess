using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject moveIndicator;

    [SerializeField] private GameObject captureIndicator;

    [SerializeField] private GameObject checkIndicator;

    [Header("Tile Colors")]

    [SerializeField] private Color lightTileColor;
    [SerializeField] private Color darkTileColor;

    [SerializeField] private Color frozenLightColor;
    [SerializeField] private Color frozenDarkColor;

    // Private Fields

    private BoardCoordinate coordinate;

    private Color defaultColor;

    private Team tileTeam;

    // Public Properties

    public BoardCoordinate Coordinate => coordinate;

    public List<TemporaryTileEffect> TemporaryEffects = new();

    public bool IsLightTile => tileTeam == Team.White;

    // Events

    public static event Action<Tile> TileClicked;

    // Unity Messages

    private void OnMouseDown()
    {
        TileClicked?.Invoke(this);
    }

    // Public Methods

    public void Initialize(BoardCoordinate coordinate, Team tileTeam)
    {
        this.coordinate = coordinate;
        this.tileTeam = tileTeam;

        UpdateVisual();
    }

    public void Highlight(bool isCapture)
    {
        moveIndicator.SetActive(!isCapture);

        captureIndicator.SetActive(isCapture);
    }

    public void ResetHighlight()
    {
        moveIndicator.SetActive(false);

        captureIndicator.SetActive(false);
    }

    public void HighlightCheck()
    {
        checkIndicator.SetActive(true);
    }

    public void ResetCheckHighlight()
    {
        checkIndicator.SetActive(false);
    }

    public void AddTemporaryEffect(TileEffect effect, int duration)
    {
        TemporaryEffects.Add(new TemporaryTileEffect(effect, duration));

        UpdateVisual();
    }

    public void ApplyEffects(BoardManager boardManager)
    {
        foreach (TemporaryTileEffect effect in TemporaryEffects)
        {
            effect.Effect.Apply(this, boardManager);
        }
    }

    public bool HasEffect<T>() where T : TileEffect
    {
        return TemporaryEffects.Exists(effect => effect.Effect is T);
    }

    public void UpdateVisual()
    {
        spriteRenderer.color = GetCurrentColor();
    }

    // Private Helpers

    private Color GetCurrentColor()
    {
        if (HasEffect<FrozenTileEffect>())
        {
            return IsLightTile ? frozenLightColor : frozenDarkColor;
        }

        return IsLightTile ? lightTileColor : darkTileColor;
    }
}