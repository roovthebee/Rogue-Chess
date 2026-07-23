using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject moveIndicator;

    [SerializeField] private GameObject captureIndicator;

    [SerializeField] private Color checkColor;

    // Private Fields

    private BoardCoordinate coordinate;

    private Color defaultColor;

    // Public Properties

    public BoardCoordinate Coordinate => coordinate;

    // Events

    public static event Action<Tile> TileClicked;

    // Public Methods

    public void Initialize(BoardCoordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;

        defaultColor = color;
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
        spriteRenderer.color = checkColor;
    }

    public void ResetCheckHighlight()
    {
        spriteRenderer.color = defaultColor;
    }

    // Unity Messages

    private void OnMouseDown()
    {
        TileClicked?.Invoke(this);
    }
}