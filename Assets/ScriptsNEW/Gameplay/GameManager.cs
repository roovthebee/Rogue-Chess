using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private BoardManager boardManager;

    [SerializeField] private ChessRules chessRules;

    [SerializeField] private CardManager cardManager;

    // Public Properties

    public static GameManager Instance { get; private set; }

    public Team CurrentTurn { get; private set; }

    public bool IsGameOver { get; private set; }

    public Team? WinningTeam { get; private set; }

    public bool WhiteInCheck { get; private set; }

    public bool BlackInCheck { get; private set; }

    // Events

    public event Action<Team> TurnChanged;

    public event Action<Team?> GameEnded;

    // Unity Messages

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            
            return;
        }

        Instance = this;
    }

    // Public Methods

    public void StartGame()
    {
        CurrentTurn = Team.White;

        IsGameOver = false;

        WinningTeam = null;

        UpdateCheckIndicator();

        cardManager.BeginTurn();

        TurnChanged?.Invoke(CurrentTurn);
    }

    public void EndTurn()
    {
        if (IsGameOver)
        {
            return;
        }

        cardManager.EndTurn();

        CurrentTurn = CurrentTurn == Team.White ? Team.Black : Team.White;

        cardManager.BeginTurn();

        UpdateCheckIndicator();

        if (chessRules.IsCheckmate(CurrentTurn))
        {
            EndGame(CurrentTurn == Team.White ? Team.Black : Team.White);

            return;
        }

        if (chessRules.IsStalemate(CurrentTurn))
        {
            EndGame(default);

            return;
        }

        TurnChanged?.Invoke(CurrentTurn);
    }

    public void EndGame(Team? winningTeam)
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;

        WinningTeam = winningTeam;

        GameEnded?.Invoke(winningTeam);
    }

    // Private Helpers

    private void UpdateCheckIndicator()
    {
        boardManager.ClearCheck();

        WhiteInCheck = chessRules.IsKingInCheck(Team.White);
        BlackInCheck = chessRules.IsKingInCheck(Team.Black);

        if (WhiteInCheck)
        {
            boardManager.ShowCheck(chessRules.GetKingCoordinate(Team.White));

            AudioManager.Instance.PlayCheck();

            return;
        }

        if (BlackInCheck)
        {
            boardManager.ShowCheck(chessRules.GetKingCoordinate(Team.Black));

            AudioManager.Instance.PlayCheck();
        }
    }
}