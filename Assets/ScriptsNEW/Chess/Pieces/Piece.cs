using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private PieceData pieceData;

    [Header("Effect Colors")]

    [SerializeField] private Color frozenColor;

    // Private Fields

    private Team team;

    private BoardCoordinate coordinate;

    private bool hasMoved;

    private SpriteRenderer spriteRenderer;

    private readonly List<TemporaryMovementRule> temporaryMovementRules = new();

    private readonly List<TemporaryPieceStatus> temporaryStatuses = new();

    // Public Properties

    public PieceData PieceData => pieceData;

    public Team Team => team;

    public BoardCoordinate Coordinate => coordinate;

    public bool HasMoved => hasMoved;

    public IEnumerable<TemporaryMovementRule> TemporaryMovementRules => temporaryMovementRules;

    // Unity Messages

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Public Methods

    public void Initialize(PieceData pieceData, Team team, BoardCoordinate coordinate)
    {
        this.pieceData = pieceData;
        this.team = team;
        hasMoved = false;

        spriteRenderer.sprite = pieceData.Sprite;

        SetCoordinate(coordinate);

        UpdateVisual();
    }

    public void SetCoordinate(BoardCoordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public void MarkAsMoved()
    {
        hasMoved = true;
    }

    public void UpdateView()
    {
        if (PlayerRoleManager.Instance.LocalTeam == Team.Black)
        {
            spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        else
        {
            spriteRenderer.transform.rotation = Quaternion.identity;
        }
    }

    public void AddTemporaryMovementRule(MovementRule rule, int duration)
    {
        temporaryMovementRules.Add(new TemporaryMovementRule(rule, duration));
    }

    public void AddTemporaryStatus(PieceStatus status, int duration)
    {
        TemporaryPieceStatus existing = temporaryStatuses.Find(s => s.Status == status);

        if (existing != null)
        {
            existing.RemainingTurns = Mathf.Max(existing.RemainingTurns, duration);
            return;
        }

        temporaryStatuses.Add(new TemporaryPieceStatus(status, duration));

        UpdateVisual();
    }

    public void RemoveTemporaryStatus(PieceStatus status)
    {
        TemporaryPieceStatus existing = temporaryStatuses.Find(s => s.Status == status);

        if (existing != null)
        {
            temporaryStatuses.Remove(existing);

            UpdateVisual();
        }
    }

    public bool HasStatus(PieceStatus status)
    {
        return temporaryStatuses.Exists(s => s.Status == status);
    }

    public IEnumerable<MovementRule> GetMovementRules()
    {
        foreach (MovementRule rule in PieceData.MovementRules)
        {
            yield return rule;
        }

        foreach (TemporaryMovementRule temporaryRule in temporaryMovementRules)
        {
            yield return temporaryRule.Rule;
        }
    }

    public void TickTemporaryMovementRules()
    {
        for (int i = temporaryMovementRules.Count - 1; i >= 0; i--)
        {
            temporaryMovementRules[i].RemainingTurns--;

            if (temporaryMovementRules[i].RemainingTurns <= 0)
            {
                temporaryMovementRules.RemoveAt(i);
            }
        }
    }

    public void TickStatuses()
    {
        for (int i = temporaryStatuses.Count - 1; i >= 0; i--)
        {
            temporaryStatuses[i].RemainingTurns--;

            if (temporaryStatuses[i].RemainingTurns <= 0)
            {
                temporaryStatuses.RemoveAt(i);

                UpdateVisual();
            }
        }
    }

    public void UpdateVisual()
    {
        spriteRenderer.color = GetCurrentColor();
    }

    // Private Helpers

    private Color GetCurrentColor()
    {
        if (HasStatus(PieceStatus.Frozen))
        {
            return frozenColor;
        }

        return Color.white;
    }
}